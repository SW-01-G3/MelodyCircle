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

//Procurar
// Função para verificar e definir os checkboxes selecionados
function checkSelectedCheckboxes() {
    const selectedValues = localStorage.getItem('selectedValues');
    if (!selectedValues) {
        document.getElementById('searchNone').checked = true;
        localStorage.setItem('selectedValues', 'None');
    } else {
        const selectedCheckboxes = selectedValues.split(',');
        selectedCheckboxes.forEach(value => {
            const checkbox = document.getElementById('search' + value);
            if (checkbox) {
                checkbox.checked = true;
            }
        });
    }
}

// Seleciona todos os checkboxes exceto o checkbox "Nenhum filtro"
const checkboxes = document.querySelectorAll('.form-check-input:not(#searchNone)');

// Adiciona um evento de mudança a cada checkbox
checkboxes.forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        // Cria um array para armazenar os valores selecionados
        const selectedValues = [];

        // Verifica cada checkbox e adiciona seu valor ao array se estiver marcado
        checkboxes.forEach(cb => {
            if (cb.checked) {
                selectedValues.push(cb.value);
            }
        });

        // Se nenhum checkbox estiver selecionado, seleciona o checkbox "Nenhum filtro"
        if (selectedValues.length === 0) {
            document.getElementById('searchNone').checked = true;
            selectedValues.push('None');
        } else if (selectedValues.includes('None')) {
            // Se o checkbox "Nenhum filtro" estiver selecionado, desmarca os outros checkboxes
            checkboxes.forEach(cb => {
                if (cb !== document.getElementById('searchNone')) {
                    cb.checked = false;
                }
            });
            selectedValues.splice(selectedValues.indexOf('None'), 1);
        }

        // Armazena os valores selecionados no armazenamento local
        localStorage.setItem('selectedValues', selectedValues.join(','));

        // Atualiza a página com a nova URL
        window.location.href = '?' + selectedValues.join('&');
    });
});

// Adiciona um evento de mudança ao checkbox "Nenhum filtro"
document.getElementById('searchNone').addEventListener('change', function () {
    if (this.checked) {
        // Se o checkbox "Nenhum filtro" for selecionado, desmarca os outros checkboxes
        checkboxes.forEach(cb => {
            cb.checked = false;
        });

        // Armazena 'None' como único valor selecionado no armazenamento local
        localStorage.setItem('selectedValues', 'None');

        // Atualiza a página com a nova URL
        window.location.href = '?None';
    }
});

// Verifica e define os checkboxes selecionados ao carregar a página
checkSelectedCheckboxes();