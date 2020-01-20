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

$(function () {
    $("#searchEventDirectorForm").submit(function (e) {
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

function refreshEventDirectors(eventId, userEmail, isPrimary) {
    if (eventId === null || eventId === undefined) {
        eventId = $("#eventSearchBox").val();
    }
    if (userEmail === null || userEmail === undefined) {
        userEmail = $("#emailSearchBox").val();
    }
    var applicationIds = $("input:checkbox[name='ApplicationIds']:checked").map(function () {
        return $(this).val();
    }).get();
    if (isPrimary === null || isPrimary === undefined) {
        isPrimary = $("#isPrimary").val();
    }
    if (eventId === "" && userEmail === "") { // && applicationIds === "") {
        toastr.warning("You must select at least one search option.");
        btnSearch.html("Search").attr("disabled", false);
        return;
    }

    $.ajax({
        url: eventDirectorSearchUrl,
        type: "POST",
        data: { EventId: eventId, UserEmail: userEmail, ApplicationIds: applicationIds, IsPrimary: isPrimary },
        dataType: "json",
        success: function (result) {
            $("#tableHolder").html(result.data);
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

$("body").on("click", ".edit-eventdirector", function (e) {
    var editBtn = $(e.currentTarget);
    var eventDirectorId = editBtn.attr("data-id");
    bindEventDirectorEditView(eventDirectorId);
});

function bindEventDirectorEditView(eventDirectorId) {
    var url = eventDirectorEditUrl + "?id=" + eventDirectorId;
    $.ajax({
        type: "GET",
        url: url
    }).success(function (result) {
        $("#editEventDirectorContainer").html(result);
        $("#editEventDirector").modal("show");
    })
        .error(function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
        });
}

$("body").on("click", ".del-eventdirector", function (e) {
    var delBtn = $(e.currentTarget);
    e.preventDefault();

    swal({
        title: "Are you sure?",
        text: "You are about to delete this event director.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then(function () {
        var innerSpan = $(delBtn.get(0).firstElementChild);
        var eventDirectorId = delBtn.attr("data-id");
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");

        $.ajax({
            url: eventDirectorDelUrl,
            data: { "id": eventDirectorId },
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Deleted!",
                        text: result.message,
                        type: "success"
                    });
                    refreshEventDirectors();
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

$("#eventSearchBox2").select2(
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

$(document).ready(function () {
    $("#emailSearchBox2").select2(
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
});

$(function () {
    $("#addEventDirectorForm").submit(function (e) {
        var btn = $("#btnAdd").html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Adding...").attr("disabled", true);
        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    btn.html("Add").attr("disabled", false);
                    swal({
                        title: result.title,
                        text: result.message,
                        type: "success",
                        allowOutsideClick: false
                    }).then(function () {
                        refreshEventDirectors();
                    },
                        function () {
                            refreshEventDirectors();
                        });
                } else if (result.success === false) {
                    swal({
                        title: result.title,
                        text: result.message,
                        type: "error"
                    });
                    btn.html("Add").attr("disabled", false);
                }
                else {
                    swal({
                        title: result.title,
                        text: result.message,
                        type: "warning"
                    }).then(function () {
                        refreshEventDirectors();
                    },
                        function () {
                            refreshEventDirectors();
                        });
                    btn.html("Add").attr("disabled", false);
                }
            },
            error: function (xhr, textStatus, error) {
                swal({
                    title: "Error!",
                    text: xhr.statusText,
                    type: textStatus
                });
                btn.html("Add").attr("disabled", false);
            }
        });
    });
});

$(document).on("submit", "#editEventDirectorForm", function (e) {
    var btn = $("#btnSave").html("<i class=\"fa fa-spinner fa-spin\" id=\"loading\"></i> Saving...").attr("disabled", true);
    e.preventDefault();
    $.ajax({
        url: this.action,
        type: this.method,
        data: $(this).serialize(),
        dataType: "json",
        success: function (result) {
            if (result.success === true) {
                btn.html("Save").attr("disabled", false);
                swal({
                    title: result.title,
                    text: result.message,
                    type: "success",
                    allowOutsideClick: false
                }).then(function () {
                    refreshEventDirectors();
                },
                    function () {
                        refreshEventDirectors();
                    });
            } else if (result.success === false) {
                swal({
                    title: result.title,
                    text: result.message,
                    type: "error"
                });
                btn.html("Save").attr("disabled", false);
            }
            else {
                swal({
                    title: result.title,
                    text: result.message,
                    type: "warning"
                }).then(function () {
                    refreshEventDirectors();
                },
                    function () {
                        refreshEventDirectors();
                    });
                btn.html("Save").attr("disabled", false);
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            btn.html("Save").attr("disabled", false);
        }
    });
});