
//Q: cases to ErrorEventHandle
//What if the fle size is too large that it cannot be fit in cache?

// there is a separate thread which constantly ajax the server for schedule
// it compares the one server gave with the one it has
// if any diffrence, it has to updat the  
// then there is a timer which constantly contacts the server

/*AminTabar Corp.: 00EB7642- 7E3E- 44B5- BC1F - F377672F6980
Ardy Corp.: A133A99F - 22EE - 42D0- A083 - 6AC9CEB904DE
Amir cars: 77A15EBF- 79F2- 4F0D- 976B- CF2E9163CC50
GET /app/api/schedule/1 HTTP/1.1
Host: www.teleboard.ir
subscription-key: 00eb7642-7e3e-44b5-bc1f-f377672f6980
*/


var oldSchedule = [];
var imageFrame;
var videoFrame;
var messagePane;
var xhr;
var contents = [];
contents.index = -1;
var blobs = [];
blobs.index = -1;
var getChannelInterval = 3000;// the interval in mili sec between each get request

var CONSTANT = {
    video_mp4: 'video/mp4',
    image_jpeg: 'image/jpeg',
    image_gif: 'image/gif',
    image_png: 'image/png',
    image_svg: 'image/svg'
}

// subscriptionKey = '77A15EBF-79F2-4F0D-976B-CF2E9163CC50';//
 //subscriptionKey = '00ec1eab-9bc9-4846-a768-c8730168df6a';//AminTabar Corp.

function cacheNextBlob() {
    blobs.index++;
    xhr = new XMLHttpRequest();
    xhr.open('GET', blobs[blobs.index].url, true);
    xhr.responseType = 'blob';
    // xhr.setRequestHeader("Cache-Control", "max-age=100000");// 100000 is too much and would not be good as it would fill up the HDD of the IOT node
    // xhr.timeout = 40000; // Set timeout to 4 seconds (4000 milliseconds)
    xhr.setRequestHeader('subscription-key', $('#SubscriptionKey').val()),
        xhr.ontimeout = function () { alert("Timed out!!!"); }
    xhr.onload = blobCached;
    xhr.onprogress = updateProgress;
    xhr.send();
    xhr.onerror = XMLHttpRequestError;
    //AA  $('#progressbar').progressbar();
    //AA xhr.abort if the prosess took so long! ..................................<<<<<<<<<<<<<<<<-----

};



function updateProgress(evt) {
    if (evt.lengthComputable) {  //evt.loaded the bytes browser receive
        //evt.total the total bytes seted by the header
        //
        var percentComplete = (evt.loaded / evt.total) * 100;
        // var divisor = evt.total > 10000000 ? 1000000 : 1000
        var divisor = 1000;
        var unit = 'KB';
        // var unit = evt.total > 10000000 ? 'MB' : 'KB'
        messagePaneUpdate((blobs.index + 1) + "/" + blobs.length + " ( " + Math.round(evt.loaded / divisor).toString().replace(/(\d)(?=(\d{3})+$)/g, "$1,") + unit + " of  " + Math.round(evt.total / divisor).toString().replace(/(\d)(?=(\d{3})+$)/g, "$1,") + unit + " )");
        //   $('#progressbar').progressbar("option", "value", percentComplete);
    }
}

/**
* initializing and bootstraping
*/

window.onload = function () {
    videoFrame = document.getElementById("videoFrame");
    initVideoFrame()
    imageFrame = document.getElementById("imageFrame");
    messagePane = document.getElementById("messagePane");
    imageFadeOut();
    videoFadeOut();
    getChannel();
}



function blobCached(e) {

    if (this.status === 200) {//If caching was a success
        blobs[blobs.index].src = (window.URL).createObjectURL(this.response);
        blobs[blobs.index].type = this.response.type;
        blobs[blobs.index].size = this.response.size;
        if (blobs.index < (blobs.length - 1)) {
            setTimeout(cacheNextBlob, 100);
        } else {
            messagePaneHide();
            // init contents with new blobs
            contents.index = -1;// may be it is not needed. let the counter continue.
            contents = [];
            for (var i = 0; i < blobs.length; ++i) {
                contents[i] = {};
                contents[i].src = blobs[i].src;
                contents[i].type = blobs[i].type;
                contents[i].duration = blobs[i].duration;
                contents[i].url = blobs[i].url;


            }
            playNextContent();// we need to do this if the playNextContent is not currently already running
        }

    } else {
        alert("Something went wrong downloading " + this.responseURL + " " + this.statusText);
    }
}

function playNextContent() {
    //AA make sure each intance runs completely before new one started
    //to avoid running this func multiple times

    if (contents.index > -1) {  // fades out previous one before
        switch (contents[contents.index].type) {
            case CONSTANT.image_jpeg:
            case CONSTANT.image_png:
            case CONSTANT.image_gif:
            case CONSTANT.image_svg:
                imageFadeOut();
                break;
            case CONSTANT.video_mp4:
                videoFadeOut();
                break;
        }

    }

    if (contents.index < (contents.length - 1)) {
        contents.index++;
    } else {
        contents.index = 0;// loop occurs
    }



    switch (contents[contents.index].type) {
        case CONSTANT.image_jpeg:
        case CONSTANT.image_png:
        case CONSTANT.image_gif:
        case CONSTANT.image_svg:
            imageFadeIn();
            playImageContent();
            break;
        case CONSTANT.video_mp4:
            //  videoFadeIn();
            playVideoContent();
            break;
    }
}


function playImageContent() {
    imageFrame.src = contents[contents.index].src;
    setTimeout(playNextContent, contents[contents.index].duration);
}

/**
 * Plays video content and calls  when video finished
 */
function playVideoContent() {
    videoFrame.style.visibility = 'hidden';
    videoFrame.type = contents[contents.index].type;
    videoFrame.src = contents[contents.index].src;
    videoFrame.play();
    videoFrame.style.visibility = 'visible';
    videoFrame.removeAttribute("controls"); 
}


function imageFadeOut() {
    imageFrame.style.visibility = 'hidden';
    imageFrame.src = '';

}
function videoFadeOut() {
    videoFrame.style.visibility = 'hidden';
    videoFrame.src = '';
}

function imageFadeIn() {
    imageFrame.style.visibility = 'visible';
}
function videoFadeIn() {
    videoFrame.style.visibility = 'visible';
}


/**
 * Gets channel in JSON format from web server
 * @param {Number} a
 */


function getChannel() {
    var schedule = [];
    data = {};// the token etc that i will send
    
    $.ajax({
        type: "Get",
        url: '/api/schedule/' + $('#DeviceId').val(),
        dataType: "json",
        headers: {
            'subscription-key': $('#SubscriptionKey').val(),//'00ec1eab-9bc9-4846-a768-c8730168df6a',
	        'datetimenow' : moment(moment.utc(moment.utc().format('YYYY-MM-DD HH:mm:ss')).toDate()).format('YYYY-MM-DD HH:mm:ss')
        },
        contentType: "application/json;charset=utf-8",
        crossDomain: true,
        success: function (schedule) {
            if (schedule != "[]" && schedule.length != 0) {
                // if no change in user's requested channel
                // carry on, do not rechache
                if (oldSchedule !== JSON.stringify(schedule)) {

                    //If there is an ongoing Ajax call, abort it 
                    if (xhr) {
                        xhr.abort();
                    }
                    blobs = [];

                    //  for (var i = 0; i < teleboard.schedule.timebox[0].channel.content.length; ++i) {
                    for (var i = 0; i < schedule.length; ++i) {
                        blobs[i] = {};
                        /*blobs[i].duration = parseInt(teleboard.schedule.timebox[0].channel.content[i].duration);
                          blobs[i].url = teleboard.schedule.timebox[0].channel.content[i].url;*/
                        blobs[i].duration = parseInt(schedule[i].Duration) * 1000;
                        blobs[i].url = schedule[i].Url;
                    }

                    {//It is a new channel, so start from begining 
                        blobs.index = -1;
                        videoFadeOut();
                        imageFadeOut();
                    }
                    cacheNextBlob();

                    oldSchedule = JSON.stringify(schedule);

                }


            } else {
                // do defualt when there is nothign to show (display your ad)
                console.log("no success");

            }

        },
        error: function (e) {
            console.log("e.statusText:" + e.statusText + " e.statusCode" + e.statusCode);
        }
    });


    // Retrieve the object from storage now that API is not ready
    var retrievedObject = localStorage.getItem('teleboardSchedule');
    /* if (retrievedObject == null) {
 
         messagePaneUpdate("Trying to connect ....");
         setTimeout(getChannel, getChannelInterval);
         return null;
     } else {
         messagePaneHide();
     }*/
    /* TODO: to remove the block
    if (retrievedObject == null) {
        retrievedObject = JSON.stringify(localschedule);
    } */


    console.log(new Date());
    // next line has to be activated ony if all blobs are loaded
    setTimeout(getChannel, getChannelInterval);
}

/**
 * Handling any already unhandled XMLHttp RequestError
 * @param {?} e
 */
function XMLHttpRequestError(e) {
    console.log("error:" + e);
}

function initVideoFrame() {
    videoFrame.controls = false;
    //videoFrame.autoplay = true;
    videoFrame.muted = true; // otherwise it won't play in Android without user intervention 
    videoFrame.onended = playNextContent;
}
/**
 * Displaying Error/info message 
 * @param {string} st
 */
function messagePaneUpdate(st) {
    messagePane.style.visibility = 'visible';
    messagePane.innerHTML = st;
}

/**
 * Hiding message pane 
 */
function messagePaneHide() {
    messagePane.style.visibility = 'hidden';
}