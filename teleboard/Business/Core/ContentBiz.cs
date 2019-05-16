using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Teleboard.Common.Configuration;
using Teleboard.Common.Data;
using Teleboard.Common.Enum;
using Teleboard.Common.Exception;
using Teleboard.Common.ExtensionMethod;
using Teleboard.Common.Media;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using Teleboard.Localization;
using Teleboard.Mapper.Core;
using Teleboard.PresentationModel.Model.Content;

namespace Teleboard.Business.Core
{
    public class ContentBiz : BizBase<Content>
    {
        private ApplicationDbContext Context { get; set; }

        public ContentBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public IEnumerable<ContentPM> ReadUnApprovedContents(ApplicationUser user)
        {
            var userId = user.Identity.GetUserId();
            var query = Context.Contents.Where(c => c.Flag == false);
            if (!user.IsHostAdmin)
                query = query.Where(c => c.Tenant.TenantUsers.Any(tu => tu.UserId == userId));
            return query.MapTo<ContentPM>().ToList();
        }

        public DataSourceResult ReadTenantContents(IPrincipal user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            var query = Context.Contents.Where(c => c.Tenant.TenantUsers.Any(tu => tu.UserId == userId));
            query = FilterContents(request, query);
            return query.OrderByDescending(c => c.Id)
                .MapTo<ContentPM>()
                .ToDataSourceResult(request);
        }


        public DataSourceResult ReadContents(DataSourceRequest request)
        {
            return FilterContents(request, Context.Contents.AsQueryable())
               .OrderByDescending(c => c.Id)
               .MapTo<ContentPM>()
               .ToDataSourceResult(request);
        }

        private static IQueryable<Content> FilterContents(DataSourceRequest request, IQueryable<Content> query)
        {
            var mimes = new List<string>();
            if (request.IncludedResourceTypes.Contains(ResourceType.Image))
                mimes.AddRange(new string[] { "image/jpeg", "image/png", "image/gif", "image/bmp" });
            if (request.IncludedResourceTypes.Contains(ResourceType.Video))
                mimes.AddRange(new string[] { "video/mp4" });
            query = query.Where(c => mimes.Contains(c.ContentType.Name.ToLower()));
            if (string.IsNullOrWhiteSpace(request.SearchValue))
                return query;
            return query.Where(c =>
                c.Description.ToLower().Contains(request.SearchValue) ||
                c.Tenant.Name.ToLower().Contains(request.SearchValue));
        }

        public DataSourceResult ReadUserContents(ApplicationUser user, DataSourceRequest request)
        {
            var userId = user.Identity.GetUserId();
            var query = Context.Contents.Where(c => c.CreatorId == userId);
            query = FilterContents(request, query);
            return query.OrderByDescending(c => c.Id)
                .MapTo<ContentPM>()
                .ToDataSourceResult(request);
        }

        public IEnumerable<ContentPM> ReadUserContents(string userId)
        {
            var contents = Context.Contents
                .Where(c => c.CreatorId == userId)
                .OrderByDescending(c => c.Id)
                .MapTo<ContentPM>()
                .ToList();
            contents.ForEach(c =>
            {
                c.Url = ComputeContentUrl(c.Source);
                c.ThumbnailUrl = ComputeContentThumbnailUrl(c.Source);
            });
            return contents;
        }

        public IEnumerable<ContentPM> ReadContents(int tenantId)
        {
            return Context.Contents
                .Where(c => c.TenantId == tenantId)
                .MapTo<ContentPM>()
                .ToList();
        }

        public ContentPM ReadContent(Guid guid)
        {
            return Context.Contents
                .Include(c => c.ContentType)
                .Single(c => c.Guid == guid)
                .GetContentPM();
        }

        public ContentPM ReadContent(string contentSource)
        {
            return Context.Contents
                .Include(c => c.ContentType)
                .Single(c => c.Source.ToLower() == contentSource.ToLower())
                .GetContentPM();
        }

        public ContentPM ReadContent(int id)
        {
            return Context.Contents
                .Include(c => c.ContentType)
                .Single(c => c.Id == id)
                .GetContentPM();
        }

        public IEnumerable<ChannelContentPM> ReadChannelContents(int channelId)
        {
            return Context.ChannelContents
                .Include(c => c.Content.ContentType)
                .Include(c => c.Channel)
                .Where(c => c.ChannelId == channelId)
                .OrderBy(c => c.Sequence)
                .MapTo<ChannelContentPM>()
                .ToList();
        }

        public IEnumerable<ChannelContentPM> ReadChannelContentsForAdvertisementPage(int channelId)
        {
            return Context.ChannelContents
                .Where(c => c.ChannelId == channelId)
                .MapTo<ChannelContentPM>()
                .ToList();
        }

        public async Task DeleteContentAsync(int id)
        {
            var content = await Context.Contents.FindAsync(id);
            Context.Contents.Remove(content);
            await Context.SaveChangesAsync();
            File.Delete(ComputeContentFilePath(content.TenantId, content.Source));
            File.Delete(ComputeContentThumbnailFilePath(content.TenantId, content.Source));
        }

        public async Task DeleteContentAsync(int id, ApplicationUser user)
        {
            if (await Context.Contents.AnyAsync(x => x.Id == id && x.Tenant.TenantUsers.Any(tu => tu.UserId == user.Id)))
            {
                await DeleteContentAsync(id);
            }
        }

        public async Task UpdateContentAsync(Guid contentGuid, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentNullException("new-description");
            var content = await Context.Contents.SingleOrDefaultAsync(c => c.Guid == contentGuid);
            if (content == null) throw new ResourceNotFountException(SysResource.ContentNotFound);
            content.Description = newDescription;
            await Context.SaveChangesAsync();
        }

        public async Task StoreContentAsync(int tenantId, Stream inputStream, string mimeType, string fileName, string description, ApplicationUser user)
        {
            var contentGuid = Guid.NewGuid();
            var contentLength = (int)inputStream.Length;
            int? videoDuration = null;
            var tenant = Context.Tenants.Find(tenantId);
            var contentType = Context.ContentTypes.Single(c => c.Name.ToLower() == mimeType.ToLower());
            var validFileName = Path.GetFileName(fileName).ToValidFileNameWithTimeStamp();
            var fileFullPath = ComputeContentFilePath(tenant.Id, validFileName);
            var thumbnailFullPath = ComputeContentThumbnailFilePath(tenant.Id, validFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath));
            inputStream.SaveAs(fileFullPath);

            if (contentType.ResourceType == ResourceType.Video)
                videoDuration = (int)MediaHelper.GetVideoDuration(fileFullPath);

            Context.Contents.Add(new Content
            {
                Guid = contentGuid,
                ContentTypeId = contentType.Id,
                Description = description,
                Source = validFileName,
                TenantId = tenant.Id,
                FileSize = contentLength,
                Flag = !tenant.ContentModeration.Value,
                Duration = videoDuration,
                CreatorId = user.Id,
            });
            await Context.SaveChangesAsync();
        }

        public string ComputeContentFilePath(int tenantId, string contentSource)
        {
            return Path.Combine(AppConfiguration.AppStoragePath, "Contents", tenantId.ToString(), contentSource);
        }

        public string ComputeContentThumbnailFilePath(int tenantId, string contentSource)
        {
            return Path.Combine(AppConfiguration.AppStoragePath, "Contents", tenantId.ToString(), contentSource.GetThumbnailName());
        }

        public string ComputeContentUrl(string contentSource)
        {
            return $"/api/v1/contents/{contentSource}";
        }

        public string ComputeContentUrl(string serverBaseUrl, string contentSource)
        {
            return Path.Combine(serverBaseUrl, "api/v1/contents", contentSource).Replace(@"\", "/");
        }

        public string ComputeContentThumbnailUrl(string contentSource)
        {
            return $"/api/v1/contents/thumbnail/{contentSource}";
        }

        public string ComputeContentThumbnailUrl(string serverBaseUrl, string contentSource)
        {
            return Path.Combine(serverBaseUrl, "api/v1/contents/thumbnail", contentSource).Replace(@"\", "/");
        }

        public IQueryable<Content> ReadAllowedToAccessContents(ApplicationUser user)
        {
            var userId = user.Identity.GetUserId();
            var query = Context.Contents.Include(c => c.ContentType);
            if (!user.IsHostAdmin)
                query = query.Where(c => c.Tenant.TenantUsers.Any(tu => tu.UserId == userId));
            return query;
        }

        public ContentPM ReadContentForApproval(int contentId, ApplicationUser user)
        {
            return ReadAllowedToAccessContents(user).Single(c => c.Id == contentId).GetContentPM();
        }
    }
}
