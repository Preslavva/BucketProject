(function () {
    document.addEventListener("DOMContentLoaded", function () {
        var stored = localStorage.getItem("lastTab");
        var hash = window.location.hash;
        var active = stored || hash;

        if (active) {
            var trigger = document.querySelector(`.nav-tabs button[data-bs-target="${active}"]`);
            if (trigger) {
                new bootstrap.Tab(trigger).show();
            }
        }

        document.querySelectorAll('.nav-tabs button[data-bs-toggle="tab"]').forEach(function (tab) {
            tab.addEventListener('shown.bs.tab', function (e) {
                var target = e.target.getAttribute('data-bs-target');
                localStorage.setItem('lastTab', target);
                window.location.hash = target;
            });
        });
    });
})();

function loadUsers(page = 1) {
    const queryString = $('#userFilterForm').serialize() + '&page=' + page;

    $.ajax({
        url: '/Manager/FilterUsersAjax',
        type: 'GET',
        data: queryString,
        success: function (result) {
            $('#userTableContainer').html(result);
        },
        error: function () {
            alert("Failed to load users. Please try again.");
        }
    });
}

$(document).on('click', '.page-link', function (e) {
    e.preventDefault();
    const selectedPage = $(this).data('page');
    if (selectedPage) {
        loadUsers(selectedPage);
    }
});

$('#userFilterForm input, #userFilterForm select').on('change input', function () {
    loadUsers(1);
});

$(document).ready(function () {
    $('input[name="searchTerm"]').on('input', function () {
        var term = $(this).val().trim();

        if (term.length === 0) {
            $('#searchResults').html('');
            $('#staticSections').show();
            return;
        }

        $.ajax({
            url: '/Social/Search',
            type: 'GET',
            data: { searchTerm: term },
            success: function (data) {
                $('#searchResults').html(data);
                $('#staticSections').hide();
            },
            error: function () {
                alert("Search failed. Please try again.");
            }
        });
    });
});

function resizeInput(input) {
    input.style.width = "1px";
    input.style.width = (input.scrollWidth + 5) + "px"; 
}

window.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.auto-resize-input').forEach(function (input) {
        resizeInput(input);
    });
});

function resizeInput(input) {
    let span = document.createElement('span');
    span.style.visibility = 'hidden';
    span.style.position = 'absolute';
    span.style.whiteSpace = 'pre';
    span.style.font = getComputedStyle(input).font;
    span.textContent = input.value || input.placeholder || '';
    document.body.appendChild(span);
    input.style.width = span.offsetWidth + 10 + 'px';
    document.body.removeChild(span);
}

window.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.auto-resize-input').forEach(function (input) {
        resizeInput(input);
    });
});