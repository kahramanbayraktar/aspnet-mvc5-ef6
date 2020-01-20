var btnSearch = $("#btnSearch");

$("#eventSearchBox").select2(
    {
        placeholder: "Find events",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: eventSearchUrl,
            dataType: "json",
            data: function (term, page) {
                return {
                    pageSize: pageSize,
                    pageNum: page,
                    searchTerm: term
                };
            },
            results: function (data, page) {
                var more = (page * pageSize) < data.Total;
                return { results: data.Results, more: more };
            }
        }
    });

$(function () {
    $("#searchEmailNotifForm").submit(function (e) {
        $("#results").loading();
        btnSearch.html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Searching...").attr("disabled", true);
        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === false) {
                    $("#results").loading("stop");
                    toastr.warning(result.message);
                } else {
                    $("#results").loading("stop");
                    $("#tableHolder").html(result.data);
                }
                btnSearch.html("Search").attr("disabled", false);
            },
            error: function (xhr, textStatus, error) {
                toastr.error(xhr.statusText);
                $("#results").loading("stop");
                btnSearch.html("Search").attr("disabled", false);
            }
        });
    });
});

//$("#btnSearch").on("click", function (e) {
//    e.preventDefault();
//    btnSearch.html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Searching...").attr("disabled", true);
//    refreshNotifications();
//});

//function refreshNotifications() {
//    var eventId = $("#eventSearchBox").val();
//    var dayRange = $("#dayRange").val();
//    //if (eventId === "" && dayRange === "") {
//    //    toastr.warning("You must select at least one search option.");
//    //    btnSearch.html("Search").attr("disabled", false);
//    //    return;
//    //}
//    $.get(notificationGetUrl + "?eventId=" + eventId + "&dayRange=" + dayRange, function (result) {
//        $("#tableHolder").html(result);
//        if (btnSearch.attr("disabled") === "disabled") {
//            btnSearch.html("Search").attr("disabled", false);
//        }
//    });
//}

$("#dayRange").change(function () {
    var dayRangeVal = this.value;
    if (dayRangeVal === "-1")
        $("#emailDateDiv").show();
    else
        $("#emailDateDiv").hide();
});