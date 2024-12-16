var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Admin/Category/Upsert/${data}" class="btn btn-info">
                        <i class="fas fa-edit"></i>
                        </a>
                        <a class="btn btn-danger"data-name="{{$categories->name}}" onclick=Delete("/Admin/Category/Delete/${data}")>
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
                type:"DELETE",
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