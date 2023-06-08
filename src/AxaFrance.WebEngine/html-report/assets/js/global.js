function openSelectedTab(idTab, idContentTab) {
    hideElement("class-container-tab");
    unSelectedAllTab("tablink");
    selectedTabById(idTab,idContentTab);
}

function selectedTabById(idTab,idContentTab){
    showElement(idContentTab);
    addClassToElement(idTab);
}

function openSelectedLineInTree() {
    let toggler = document.getElementsByClassName("caret");

    for (const togglerElement of toggler) {
        togglerElement.addEventListener("click", function() {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }
}

function showElement(idTab){
    document.getElementById(idTab).style.display = "block";
}

function hideElement(className){
    let tabArray = document.getElementsByClassName(className);
    for (const tabArrayElement of tabArray) {
        tabArrayElement.style.display = "none";
    }
}

function unSelectedAllTab(className){
    let i;
    let tablinks = document.getElementsByClassName(className);
    for (const tablink of tablinks) {
        tablink.className = tablink.className.replace(" w3-border-blue", "");
    }
}

function addClassToElement(idTab) {
    document.getElementById(idTab).classList.add("w3-border-blue");
}

function changeColorOfSelectedElement(event){
    let tablinks = document.getElementsByClassName("selected-line");
    for (const tablink of tablinks) {
        tablink.className = tablink.className.replace("selected-line", "");
    }
    event.currentTarget.className += " selected-line";
}

function displayImage(imgToDisplay){
    let modal = document.getElementById("myModal");
    let img = document.getElementById(imgToDisplay);
    let modalImg = document.getElementById("img01");
    let captionText = document.getElementById("caption");

    modal.style.display = "block";
    modalImg.src = img.src;
    captionText.innerHTML = img.alt;

    let span = document.getElementsByClassName("close")[0];
    span.onclick = function() {
        modal.style.display = "none";
    }
}
