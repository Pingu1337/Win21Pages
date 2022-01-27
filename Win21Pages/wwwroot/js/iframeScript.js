
var GoBackBox = document.getElementById("GoBackBox");
var iframe = document.getElementById("frame");
var iText = document.getElementById("IntroText");

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


let HaveSeenBanner = getCookie("HaveSeenBanner");

console.log(HaveSeenBanner);



async function DisplayBanner() {

    iText.style.display = "block";

    document.cookie = "HaveSeenBanner=true;";
}


if (!HaveSeenBanner) {
    DisplayBanner();
}




async function logKeys() {
    await sleep(1000);
    iframe.contentDocument.addEventListener("keydown", (evt) => {
        if (evt.shiftKey && evt.keyCode == 8) {
            GoBackBox.classList.toggle("HideBox");
        }
    });
}

logKeys();



