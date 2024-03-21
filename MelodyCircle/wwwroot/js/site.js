// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Enable tooltips in page
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})


    function previewImage() {
        var preview = document.getElementById('previewImage');
        var file = document.getElementById('profilePicture').files[0];
        var reader = new FileReader();

        reader.onloadend = function () {
            preview.src = reader.result;
        }

        if (file) {
            reader.readAsDataURL(file);
        } else {
            preview.src = ""; 
        }
}



document.addEventListener('DOMContentLoaded', function () {
    const toggleFiltersButton = document.getElementById('toggleFilters');
    const filterContent = document.querySelector('.filter-content');

    toggleFiltersButton.addEventListener('click', function () {
        filterContent.classList.toggle('active');
        // Alterna entre a exibição e a ocultação do conteúdo dos filtros ao clicar no botão de alternância
        if (toggleFiltersButton.innerHTML === '▲') {
            toggleFiltersButton.innerHTML = '▼';
        } else {
            toggleFiltersButton.innerHTML = '▲';
        }
    });
});