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

$("#emailSearchBox").select2(
    {
        placeholder: "Find users by email",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: userSearchUrl,
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

$("#countrySearchBox").select2(
    {
        placeholder: "Find countries",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: countrySearchUrl,
            dataType: "json",
            data: function (term, page) {
                $("#citySearchBox").select2("val", "");
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

$("#citySearchBox").select2(
    {
        placeholder: "Find cities",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            quietMillis: 150,
            url: function() {
                return citySearchUrl + "?countryCode=" + $("#countrySearchBox").val();
            },
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
    $("#searchEditionForm").submit(function (e) {
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

function refreshEditions(eventId, directorEmail, isPrimary) {
    if (eventId === null || eventId === undefined) {
        eventId = $("#eventSearchBox").val();
    }
    if (directorEmail === null || directorEmail === undefined) {
        directorEmail = $("#emailSearchBox").val();
    }
    var appIds = $("input:checkbox[name='ApplicationId']:checked").map(function () {
        return $(this).val();
    }).get();
    if (isPrimary === null || isPrimary === undefined) {
        isPrimary = $("#isPrimary").val();
    }
    if (eventId === "" && directorEmail === "") { // && appIds === "") {
        toastr.warning("You must select at least one search option.");
        btnSearch.html("Search").attr("disabled", false);
        return;
    }

    $.ajax({
        url: editionSearchUrl,
        type: "POST",
        data: { EventId: eventId, directorEmail: directorEmail, ApplicationId: appIds, IsPrimary: isPrimary },
        dataType: "html",
        success: function (result) {
            $("#tableHolder").html(result);
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
        }
    });
}

$("body").on("click", ".view-edition", function (e) {
    var detailsBtn = $(e.currentTarget);
    var editionId = detailsBtn.attr("data-id");
    bindEditionDetails(editionId);
});

function bindEditionDetails(editionId) {
    var url = editionGetDetailsUrl + "?id=" + editionId;
    $.ajax({
            type: "GET",
            url: url
        }).success(function (result) {
            $("#editionDetailsContainer").html(result);
            $("#editionDetails").modal("show");
        })
        .error(function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
        });
}

$("body").on("click", ".del-edition", function (e) {
    var delBtn = $(e.currentTarget);
    e.preventDefault();

    swal({
        title: "Are you sure?",
        text: "You are about to delete this edition.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then(function () {
        var innerSpan = $(delBtn.get(0).firstElementChild);
        var editionId = delBtn.attr("data-id");
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");

        $.ajax({
            url: editionDelUrl,
            data: { "id": editionId },
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Deleted!",
                        text: result.message,
                        type: "success"
                    });
                    refreshEditions();
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
    }).done();
});