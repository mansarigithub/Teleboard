
String.prototype.toPersionNumber = function () {
    let persianNumbers = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
    let chars = this.split('');
    for (var i = 0; i < chars.length; i++) {
        if (/\d/.test(chars[i])) {
            chars[i] = persianNumbers[chars[i]];
        }
    }
    return chars.join('');
}

String.prototype.toEnglishNumber = function () {
    var charMap = [{ pattern: '۰', replace: '0' }, { pattern: '۱', replace: '1' },
    { pattern: '۲', replace: '2' }, { pattern: '۳', replace: '3' },
    { pattern: '۴', replace: '4' }, { pattern: '۵', replace: '5' },
    { pattern: '۶', replace: '6' }, { pattern: '۷', replace: '7' },
    { pattern: '۸', replace: '8' }, { pattern: '۹', replace: '9' },
    ];
    let value = this;

    for (var i = 0; i < charMap.length; i++)
        value = value.replace(new RegExp(charMap[i].pattern, 'gi'), charMap[i].replace);
    return value;
}