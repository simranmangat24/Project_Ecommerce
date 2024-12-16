var dataTable;

$(document).ready(function () {
    laodDataTable();
})


function laodDataTable() {
    dataTable = $('#tbldata').DataTable({
        "aLengthMenu": [[5, 10, 15, -1], [5, 10, 15, "All"]],
        "ajax": {
            "url": "/Customer/Home/PrivacyAPI"
        },
        "columns": [
            {
                "data":null,
                "render": function (data) {
                    return `
                    <div class="col-12"  >
                            <div class="col-12">
                                <div class="col-12 p-1" style="border:1px solid #008cba; border-radius: 5px;">
                                    <div class="card col-lg-12"   style="border:0px;">
                                        <img src="${data.imageUrl}" class="card-img-top rounded" style="height:200px;" />
                                        <div class="pl-1">
                                         <p class="card-title h5"><b style="color:#2c3e50">${data.title}</b></p>
                                            <p class="card-title text-primary">by <b>${data.author} </b></p>
                                     </div>
                                        <div style="padding-left:5px;">
                                            <a>List Price: <strike><b>
                                                    <i class="fa fa-rupee" style="font-size:15px"></i>
                                                ${data.price}</b></strike>
                                            </a>
                                        </div>
                                        <div style="padding-left:5px;">
                                            <p style="color:maroon">As low as: <b class="">
                                                    <i class="fa fa-rupee" style="font-size:15px"></i>
                                                ${data.price100}</b></p>
                                        </div>
                                    </div>
                                    <div>
                                        <a  href="/Customer/Home/Details/${data.id}"  class="btn btn-primary form-control" >Details</a>
                                    </div>
                            </div>
                        </div>
                    </div>
                    `;
                }

            }
        ]

    })
}
