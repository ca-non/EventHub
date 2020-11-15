// Prime the All link 
function setupFunc() {
    const allLink = document.getElementById('all-link');
    if (allLink != null) {
        allLink.click();
        allLink.focus();
        allLink.classList.add("all-link-style");
    }

    const allLinkBrowse = document.getElementById('all-link-browse');
    if (allLinkBrowse != null) {
        allLinkBrowse.click();
        allLinkBrowse.focus();
        allLinkBrowse.classList.add("browseLink-Clicked");
    }

}


setupFunc();

// Styles for the event links
const all = document.getElementById('all-link');
const festivals = document.getElementById('festivals-link');
const foodanddrink = document.getElementById('foodanddrink-link');
const art = document.getElementById('art-link');
const movies = document.getElementById('movies-link');
const gaming = document.getElementById('gaming-link');
const sports = document.getElementById('sports-link');
const public = document.getElementById('public-link');


var links = [all, festivals, foodanddrink, art, movies, gaming, sports, public];


function allClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    all.classList.add("linked-clicked");
}

function festivalsClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    festivals.classList.add("linked-clicked");
}

function foodanddrinkClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    foodanddrink.classList.add("linked-clicked");
}


function artClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    art.classList.add("linked-clicked");
}


function moviesClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    movies.classList.add("linked-clicked");
}


function gamingClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    gaming.classList.add("linked-clicked");
}


function sportsClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    sports.classList.add("linked-clicked");
}


function publicClickHandler() {
    for (i = 0; i < links.length; i++) {

        links[i].classList.remove("linked-clicked");
    }
    public.classList.add("linked-clicked");
}


if (all != null && festivals != null && foodanddrink != null && art != null && movies != null && gaming != null && sports != null && public != null) {
    all.addEventListener('click', allClickHandler);
    festivals.addEventListener('click', festivalsClickHandler);
    foodanddrink.addEventListener('click', foodanddrinkClickHandler);
    art.addEventListener('click', artClickHandler);
    movies.addEventListener('click', moviesClickHandler);
    gaming.addEventListener('click', gamingClickHandler);
    sports.addEventListener('click', sportsClickHandler);
    public.addEventListener('click', publicClickHandler);
}

