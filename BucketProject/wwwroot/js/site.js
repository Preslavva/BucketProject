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

function loadUsers() {
    $.ajax({
        url: '/Manager/FilterUsersAjax',
        type: 'GET',
        data: $('#userFilterForm').serialize(),
        success: function (result) {
            $('#userTableContainer').html(result);
        },
        error: function () {
            alert("Failed to load users. Please try again.");
        }
    });
}

$(document).ready(function () {
    $('#userFilterForm input, #userFilterForm select').on('change input', function () {
        loadUsers();
    });
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
