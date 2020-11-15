// Styles for the event links
const all_l = document.getElementById('all-link-browse');
const festivals_l = document.getElementById('festivals-link-browse');
const foodanddrink_l = document.getElementById('foodanddrink-link-browse');
const art_l = document.getElementById('art-link-browse');
const movies_l = document.getElementById('movies-link-browse');
const gaming_l = document.getElementById('gaming-link-browse');
const sports_l = document.getElementById('sports-link-browse');
const public_l = document.getElementById('public-link-browse');


var links_l = [all_l, festivals_l, foodanddrink_l, art_l, movies_l, gaming_l, sports_l, public_l];

function all_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    all_l.classList.add("browseLink-style");
}

function festivals_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    festivals_l.classList.add("browseLink-style");
}

function foodanddrink_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    foodanddrink_l.classList.add("browseLink-style");
}


function art_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    art_l.classList.add("browseLink-style");
}


function movies_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    movies_l.classList.add("browseLink-style");
}


function gaming_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    gaming_l.classList.add("browseLink-style");
}


function sports_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    sports_l.classList.add("browseLink-style");
}


function public_lClickHandler() {
    for (i = 0; i < links_l.length; i++) {

        links_l[i].classList.remove("browseLink-style");
    }
    public_l.classList.add("browseLink-style");
}

if (all_l != null && festivals_l != null && foodanddrink_l != null && art_l != null && movies_l != null && gaming_l != null && sports_l != null && public_l != null) {
    console.log("babee");
    all_l.addEventListener('click', all_lClickHandler);
    festivals_l.addEventListener('click', festivals_lClickHandler);
    foodanddrink_l.addEventListener('click', foodanddrink_lClickHandler);
    art_l.addEventListener('click', art_lClickHandler);
    movies_l.addEventListener('click', movies_lClickHandler);
    gaming_l.addEventListener('click', gaming_lClickHandler);
    sports_l.addEventListener('click', sports_lClickHandler);
    public_l.addEventListener('click', public_lClickHandler);
}

