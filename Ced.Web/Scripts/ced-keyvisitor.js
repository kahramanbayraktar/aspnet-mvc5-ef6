$("#addkeyvisitortbn").on("click", function (e) {
    e.preventDefault();

    var addBtn = $("#addkeyvisitortbn").html("Adding...").attr("disabled", true);

    var keyVisitorId = $("#keyVisitorIdSelect").val();
    var keyVisitorValue = $("#keyVisitorValue").val();

    if (keyVisitorValue === "") {
        addBtn.html("Add").attr("disabled", false);
        swal({
            title: "Error!",
            text: "KeyVisitor Value cannot be empty",
            type: "error"
        });
        return;
    }

    $.ajax({
        url: editionKeyVisitorAddUrl,
        type: "GET",
        data: { "editionId": editionId, "keyVisitorId": keyVisitorId, "value": keyVisitorValue },
        dataType: "json",
        success: function (result) {
            if (result.success === true) {
                refreshEditionKeyVisitors(editionId);
                swal({
                    title: "Added!",
                    text: result.message,
                    type: "success"
                });
                addBtn.html("Add").attr("disabled", false);
                $("#keyVisitorValue").val("");
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                addBtn.html("Add").attr("disabled", false);
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            addBtn.html("Add").attr("disabled", false);
        }
    });
});

$("body").on("click", ".del-editionkeyvisitor", function (e) {
    e.preventDefault();
    var delBtn = $(e.currentTarget);
    var innerSpan = $(delBtn.get(0).firstElementChild);
    delBtn.addClass("disabled").css("pointer-events", "none");
    innerSpan.addClass("fa-spin");
    var editionKeyVisitorId = delBtn.attr("data-id");
    var editionId = delBtn.attr("data-editionid");
    $.ajax({
        url: editionKeyVisitorDelUrl,
        data: { editionKeyVisitorId: editionKeyVisitorId },
        dataType: "json",
        success: function (result) {
            if (result.success === true) {
                refreshEditionKeyVisitors(editionId);
                swal({
                    title: "Deleted!",
                    text: result.message,
                    type: "success"
                });
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                delBtn.removeClass("disabled");
                innerSpan.removeClass("fa-spin");
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            delBtn.removeClass("disabled");
            innerSpan.removeClass("fa-spin");
        }
    });
});

function refreshEditionKeyVisitors(editionId) {
    $.get(editionKeyVisitorGetUrl + "?editionId=" + editionId, function (result) {
        $("#divEditionKeyVisitors").html(result);
    });
}

//var pageSize = 15;

//$("#keyVisitorSearchBox").select2(
//    {
//        placeholder: "Find keyvisitor keys",
//        minimumInputLength: 3,
//        allowClear: true,
//        ajax: {
//            quietMillis: 150,
//            url: keyVisitorSearchUrl,
//            dataType: "json",
//            data: function (term, page) {
//                return {
//                    pageSize: pageSize,
//                    pageNum: page,
//                    searchTerm: term
//                };
//            },
//            results: function (data, page) {
//                var more = (page * pageSize) < data.Total;
//                return { results: data.Results, more: more };
//            }
//        }
//    });

//$("#keyVisitorSearchBox").on("select2-selecting", function (e) {
//    addEditionKeyVisitor(editionId, e.val);
//});

//function refreshEditionKeyVisitorsRoot() {
//    refreshEditionKeyVisitors(editionId);
//}

//function clearKeyVisitorSearchBox() {
//    $("#keyVisitorSearchBox").select2("val", "");
//}