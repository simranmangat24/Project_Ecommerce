var dataTable;

$(document).ready(function () {
    laodDataTable();
})

function laodDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name" },
            { "data": "email" },
            { "data": "phoneNumber" },
            { "data": "company.name" },
            { "data": "role" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockOut = new Date(data.lockoutEnd).getTime();
                    if (lockOut > today) {
                        return `
                            <div class="text-center">
                                <a class="btn btn-danger" onclick=LockUnlock('${data.id}') data-toggle="tooltip"  title="Unlock User">
                                    <i class="fas fa-user-lock"></i>
                                </a>
                            </div>
                        `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a class="btn btn-success" onclick=LockUnlock('${data.id}') data-toggle="tooltip"  title="Lock User" >
                                    <i class="fas fa-lock-open"></i>
                                </a>
                            </div>

                        `;
                    }
                }
            }
        ]
    })
}

function LockUnlock(id) {
    //alert(id);
    $.ajax({
        url: "/Admin/User/LockUnlock",
        type: "POST",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })
}