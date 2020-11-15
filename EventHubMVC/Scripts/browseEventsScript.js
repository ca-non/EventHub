
function shortText() {
    const descriptionPara = document.getElementsByClassName('browseEventParagraph');

    var words;
    var newLin;

    if (descriptionPara != null) {
        for (var i = 0; i < descriptionPara.length; i++) {
            words = descriptionPara[i].innerHTML.trim().split(" ");
            newLine = "";

            if (words.length > 65) {
                for (var j = 0; j < 65; j++) {
                    newLine += words[j] + " ";
                }
                newLine += ".....";

                descriptionPara[i].innerHTML = newLine;
            }
        }
    }

    if (descriptionPara == null) {
        console.log("hel-lo-lo");
    }

}


