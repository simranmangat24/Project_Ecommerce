var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetApprovedOrders"
        },
        "columns": [
            { "data": "orderDate" },
            { "data": "name" },
            { "data": "streetAddress" },
            { "data": "state" },
            { "data": "city" },
            { "data": "orderTotal" },
            {
                "data": null,
                "render": function (data) {
                    return `
                    
                        <div class="text-center">00
                            <a onclick="openModal('/Admin/Order/MyModal/${data}')"  class="btn btn-info">
                                <i class="fas fa-eye"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    })
}

function openModal() {
    document.getElementById("myModal").style.display = "block";
}
