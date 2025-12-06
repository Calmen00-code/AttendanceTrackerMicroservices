$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    var userId = document.getElementById("userId").value;
    if (userId)
    {
        $('#dailyRecordTable').DataTable({
            "ajax": {
                "url": "/QR/GetUserDailyRecords?userId=" + userId,
                "type": "GET"
            },
            "columns": [
                { data: 'checkIn', width: '30%' },
                { data: 'checkOut', width: '40%' },
            ]
        });
    }
    else
    {
        alert("User ID is not provided.");
    }
}
