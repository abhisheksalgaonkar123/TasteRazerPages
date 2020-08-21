var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/foodtype",
            "type": "GET",
            "datatype": "json",
            "dataSrc": ""
        },
        "columns": [
            { "data": "name", "width": "60%" },
            
            {
                "data": "id",
                "render": function (data) {
                    return ` <div class="text-center">
                                <a href="/Admin/foodtype/upsert?id=${data}" class="btn btn-success text-white" style="cussor:pointer; width:100px;">
                                    <i class="far fa-edit"></i> Edit
                                </a>
                                 <a class="btn btn-danger text-white" style="cursor:pointer; width:100px;" onclick=Delete('/api/foodtype/'+${data})>
                                    <i class="far fa-trash-alt"></i> Delete
                                </a>
                    </div>`;
                }, "width": "40%"
            }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}

