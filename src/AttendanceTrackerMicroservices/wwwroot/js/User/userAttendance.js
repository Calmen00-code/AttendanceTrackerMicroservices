$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    var userId = document.getElementById("userId").value;
    if (userId)
    {
        $('#userAttendanceTable').DataTable({
            "ajax": {
                "url": "#",
                "type": "GET"
            },
            "columns": [
                { data: 'date', width: '50%' },
                { data: 'totalWorkingHours', width: '50%' },
            ]
        });
    }
    else
    {
        alert("User ID is not provided.");
    }
}
