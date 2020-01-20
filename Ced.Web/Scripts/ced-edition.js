/* WEBLOGO & PEOPLE IMAGES */
$(document).on("click", ".deletePic", function (e) {
    var url = this.href;
    swal({
        title: "Are you sure?",
        text: "You cannot undo this action.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then
        (function () {
            var delBtn = $(e.currentTarget);
            var innerSpan = $(delBtn.get(0).firstElementChild);
            delBtn.addClass("disabled");
            innerSpan.addClass("fa-spin");
            $.ajax({
                type: "POST",
                url: url,
                success: function (result) {
                    if (result.success === true) {
                        $("#" + result.imgId).attr("src", result.defImgPath);
                        $("#" + result.delBtnId).hide();
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
                    }
                    delBtn.removeClass("disabled");
                    innerSpan.removeClass("fa-spin");
                },
                error: function () {
                    swal({
                        title: "Error!",
                        text: "Unknown error!",
                        type: "error"
                    });
                    delBtn.removeClass("disabled");
                    innerSpan.removeClass("fa-spin");
                }
            });
        });
    return false;
});
/* TAB - GENERAL INFO */
$(function () {
    $("#editGeneralInfoForm").submit(function (e) {
        var btn = $(".btnSave1").html("Saving...").attr("disabled", true);
        e.preventDefault();
        emptyToZero(".touchspin");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    if (editionTranslationId === 0) {
                        swal({
                            title: "Saved!",
                            text: result.message,
                            type: "success"
                        });
                        location.reload();
                    } else {
                        swal({
                            title: "Success!",
                            text: result.message,
                            type: "success"
                        });
                        btn.html("Save").attr("disabled", false);
                        //goToNext(btn);
                    }
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    toastr.error(result.message);
                    btn.html("Save").attr("disabled", false);
                }
            },
            error: function (xhr, textStatus, error) {
                swal({
                    title: "Error!",
                    text: xhr.statusText,
                    type: textStatus
                });
                toastr.error(xhr.statusText);
                btn.html("Save").attr("disabled", false);
            }
        });
        return false;
    });
});
$(document).ready(function () {
    $(".maxlength").maxlength({
        alwaysShow: true
    });
});
$(function () {
    $("#VenueAddress").keydown(function (e) {
        if (e.keyCode === 13) {
            $("#btnGetByAddress").focus().click();
            return false;
        }
    });
});
$(document).ready(function () {
    if (currentCoords === "") {
        $("#VenueAddress").val(eventCity);
        $("#btnGetByAddress").click();
    }
});
/* TAB - SALES METRICS */
$(function () {
    $("#editSalesMetricsForm").submit(function (e) {
        var btn = $(".btnSave2").html("Saving...").attr("disabled", true);
        e.preventDefault();
        emptyToZero(".touchspin");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Saved!",
                        text: result.message,
                        type: "success"
                    });
                    btn.html("Save").attr("disabled", false);
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    toastr.error(result.message);
                    btn.html("Save").attr("disabled", false);
                }
            },
            error: function () {
                swal({
                    title: "Error!",
                    text: "Unknown error!",
                    type: "error"
                });
                toastr.error("Unknown error!");
                btn.html("Save").attr("disabled", false);
            }
        });
        return false;
    });
});
/* TAB - EXHIBITOR & VISITOR STATS */
$(function () {
    $("#editExhibitorVisitorStatsForm").submit(function (e) {
        var btn = $(".btnSave3").html("Saving...").attr("disabled", true);
        e.preventDefault();
        emptyToZero(".touchspin");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Saved!",
                        text: result.message,
                        type: "success"
                    });
                    refreshCohostsRoot();
                    clearCohostSearchBox();
                    btn.html("Save").attr("disabled", false);
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    toastr.error(result.message);
                    btn.html("Save").attr("disabled", false);
                }
            },
            error: function () {
                swal({
                    title: "Error!",
                    text: "Unknown error!",
                    type: "error"
                });
                toastr.error("Unknown error!");
                btn.html("Save").attr("disabled", false);
            }
        });
        return false;
    });
});
$(function () {
    $(".select2").select2({ width: "100%", placeholder: "Select country" });
});
/* TAB - FILE LIBRARY */
$(document).on("click", ".deleteFile", function (e) {
    var url = this.href;
    swal({
        title: "Are you sure?",
        text: "You cannot undo this action.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then(function () {
        var delBtn = $(e.currentTarget);
        var innerSpan = $(delBtn.get(0).firstElementChild);
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");
        $.ajax({
            type: "POST",
            url: url,
            success: function (result) {
                if (result.success === true) {
                    refreshFiles(result.fileType);
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
    return false;
});
function refreshFiles(divName) {
    var divFiles = $("#div" + divName + "files");
    var url = divFiles.attr("url");
    $.get(url, function (data) {
        divFiles.html(data);
    });
};
$(function () {
    $("#divexhibitionphotofiles").on("click", ".pop", function (e) {
        e.preventDefault();
        $(".imagepreview").attr("src", $(this).find("img").attr("src"));
        $("#imagemodal").modal("show");
    });
});
/* TAB - SURVEY RESULTS (POSTSHOW METRICS) */
$(function () {
    $("#editPostShowMetricsForm").submit(function (e) {
        var btn = $(".btnSave4").html("Saving...").attr("disabled", true);
        e.preventDefault();
        emptyToZero(".touchspin");
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Saved!",
                        text: result.message,
                        type: "success"
                    });
                    btn.html("Save").attr("disabled", false);
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    toastr.error(result.message);
                    btn.html("Save").attr("disabled", false);
                }
            },
            error: function () {
                swal({
                    title: "Error!",
                    text: "Unknown error!",
                    type: "error"
                });
                toastr.error("Unknown error!");
                btn.html("Save").attr("disabled", false);
            }
        });
        return false;
    });
});
/* SUBSCIPTION */
var subscribebtn = $("#subscribebtn");
var unsubscribebtn = $("#unsubscribebtn");
$(document).on("click", "#subscribebtn", function (e) {
    e.preventDefault();
    subscribebtn.attr("disabled", true);
    var url = this.href;
    $.ajax({
        url: url,
        type: "POST",
        success: function (result) {
            if (result.success === true) {
                swal({
                    title: "Subscribed!",
                    text: result.message,
                    type: "success"
                });
                toastr.success(result.message);
                subscribebtn.hide();
                unsubscribebtn.show().attr("disabled", false);
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                toastr.error(result.message);
                subscribebtn.attr("disabled", false);
            }
        },
        error: function () {
            swal({
                title: "Error!",
                text: "Unknown error!",
                type: "error"
            });
            toastr.error("Unknown error!");
            subscribebtn.attr("disabled", false);
        }
    });
});

/* UNSUBSCIPTION */
$(document).on("click", "#unsubscribebtn", function (e) {
    e.preventDefault();
    unsubscribebtn.attr("disabled", true);
    var url = this.href;
    $.ajax({
        url: url,
        type: "POST",
        success: function (result) {
            if (result.success === true) {
                swal({
                    title: "Unsubscribed!",
                    text: result.message,
                    type: "success"
                });
                unsubscribebtn.hide();
                subscribebtn.show().attr("disabled", false);
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                unsubscribebtn.attr("disabled", false);
            }
        },
        error: function () {
            swal({
                title: "Error!",
                text: "Unknown error!",
                type: "error"
            });
            unsubscribebtn.attr("disabled", false);
        }
    });
});