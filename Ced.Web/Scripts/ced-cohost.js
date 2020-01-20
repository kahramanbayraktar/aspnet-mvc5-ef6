function addCohost(editionId, cohostId) {
    $.ajax({
        url: cohostAddUrl,
        data: { "editionId": editionId, "cohostEditionId": cohostId },
        dataType: "json",
        success: function (result) {
            if (result.success === true) {
                refreshCohosts(editionId);
                swal({
                    title: "Co-hosted!",
                    text: result.message,
                    type: "success"
                });
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            toastr.error(xhr.statusText);
        }
    });
}

function refreshCohosts(editionId) {
    $.get(cohostGetUrl + "?editionId=" + editionId, function(result) {
        $("#divCohostEditions").html(result);
    });
}

$(document).ready(function() {
    $("body").on("click", ".del-cohost", function(e) {
        e.preventDefault();
        var delBtn = $(e.currentTarget);
        var innerSpan = $(delBtn.get(0).firstElementChild);
        var cohostId = delBtn.attr("data-id");
        var editionId = delBtn.attr("data-editionId");
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");
        $.ajax({
            url: cohostDelUrl,
            data: { "cohostEditionId": cohostId },
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    refreshCohosts(editionId);
                    if (result.cohostCount === 0) {
                        $("#CohostedEvent").iCheck("uncheck");
                    } else if (result.cohostCount === 1) {
                        $("#CohostedEvent").iCheck("check");
                    }
                    swal({
                        title: "Removed!",
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
});

$("#CohostedEvent").on("ifChanged",
    function() {
        if (this.checked) {
            $("#divCohostEditionSearch").show();
        } else {
            $("#divCohostEditionSearch").hide();
        }
    });

var pageSize = 15;

$("#cohostedEditionSearchBox").select2(
    {
        placeholder: "Find co-hosts",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: editionSearchUrl,
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

$("#cohostedEditionSearchBox").on("select2-selecting", function (e) {
    addCohost(editionId, e.val);
});

function refreshCohostsRoot() {
    refreshCohosts(editionId);
}

function clearCohostSearchBox() {
    $("#cohostedEditionSearchBox").select2("val", "");
}