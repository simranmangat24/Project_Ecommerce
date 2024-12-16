var dataTable;

$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "20%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorised","width":"15%",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" checked disabled />`;
                    }
                    else {
                        return `<input type="ckeckbox"  disabled/>`;
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Company/Upsert/${data}" class="btn btn-success">
                                <i class="fas fa-edit"></i>
                            </a>

                            <a onclick=Delete("/Admin/Company/Delete/${data}") class="btn btn-danger">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    })
}
function Delete(url) {
    //alert(url);
    swal({
        title: "Are You Sure ?",
        text: "Delete",
        icon: "error",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}