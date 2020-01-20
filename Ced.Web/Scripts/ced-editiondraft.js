var saveButtons = $(".saveDraft");
var approvalButtons = $(".sendForApproval");
var approveButtons = $(".approve");
var rejectButtons = $(".reject");

approvalButtons.on("click", function (e) {
    e.preventDefault();
    var editionId = $(e.currentTarget).attr("data-id");
    swal({
        title: "Sending for Approval",
        text: "Draft event is being sent for approval...",
        imageUrl: "https://itedata.blob.core.windows.net/ced/swal-loading.png",
        imageClass: "fa-spin",
        showConfirmButton: false,
        allowOutsideClick: false
    });
    sendForApproval(e, editionId);
});

approveButtons.on("click", function (e) {
    e.preventDefault();
    var editionId = $(e.currentTarget).attr("data-id");
    swal({
        title: "Approving",
        text: "Draft event is being approved...",
        imageUrl: "http://itedata.blob.core.windows.net/ced/swal-loading.png",
        imageClass: "fa-spin",
        showConfirmButton: false,
        allowOutsideClick: false
    });
    approveDraft(e, editionId);
});

rejectButtons.on("click", function (e) {
    e.preventDefault();
    var editionId = $(e.currentTarget).attr("data-id");
    swal({
        title: "Rejecting",
        text: "Reason for rejection:",
        input: "textarea",
        type: "info",
        showCancelButton: true,
        preConfirm: function (text) {
            return new Promise(function (resolve, reject) {
                if (text === "") {
                    reject("You must enter a reason.");
                } else {
                    resolve(text);
                }
            });
        }
    }).then(function (reason) {
        rejectButtons.text("Rejecting...");
        approveButtons.attr("disabled", true);
        swal({
            title: "Rejecting",
            text: "Draft event is being rejected...",
            imageUrl: "http://itedata.blob.core.windows.net/ced/swal-loading.png",
            imageClass: "fa-spin",
            showConfirmButton: false,
            allowOutsideClick: false
        });
        rejectDraft(e, editionId, reason);
    });
});

/*SAVE DRAFT*/
$(function () {
    $("#saveDraftForm").submit(function (e) {
        saveButtons.html("<i class='fa fa-spinner fa-spin'></i> Saving Draft...").attr("disabled", true);
        approvalButtons.attr("disabled", true);
        approveButtons.attr("disabled", true);
        rejectButtons.attr("disabled", true);
        e.preventDefault();

        swal({
            title: "Saving Draft",
            text: "Draft event is being saved...",
            imageUrl: "http://itedata.blob.core.windows.net/ced/swal-loading.png",
            imageClass: "fa-spin",
            showConfirmButton: false,
            allowOutsideClick: false
        });

        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    saveButtons.html("<i class='fa fa-check'></i> Draft Saved");
                    swal({
                        title: "Saved!",
                        text: result.message,
                        type: "success",
                        allowOutsideClick: false
                    }).then(function () {
                            location.href = result.returnUrl;
                        },
                        function () {
                            location.href = result.returnUrl;
                        });
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
                    });
                    toastr.error(result.message);
                    OnError_SaveDraft();
                }
            },
            error: function (xhr, textStatus, error) {
                swal({
                    title: "Error!",
                    text: xhr.statusText,
                    type: textStatus
                });
                toastr.error(xhr.statusText);
                OnError_SaveDraft();
            }
        });
    });
});

function OnError_SaveDraft() {
    saveButtons.html("Save Draft").attr("disabled", false);
    approvalButtons.attr("disabled", false);
    approveButtons.attr("disabled", false);
    rejectButtons.attr("disabled", false);
}

/*SEND FOR APPROVAL*/
function sendForApproval(e, editionId) {
    saveButtons.attr("disabled", true);
    approvalButtons.html("<i class='fa fa-spinner fa-spin'></i> Sending For Approval...").attr("disabled", true);
    $.ajax({
        url: e.target.href,
        type: "POST",
        dataType: "json",
        data: { "id": editionId },
        success: function (result) {
            if (result.success === true) {
                approvalButtons.html("<i class='fa fa-check'></i> Sent For Approval");
                swal({
                    title: "Sent for Approval!",
                    text: result.message,
                    type: "success",
                    allowOutsideClick: false
                }).then(function() {
                        location.href = result.returnUrl;
                    },
                    function() {
                        location.href = result.returnUrl;
                    });
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                toastr.error(result.message);
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
};

/*APPROVE DRAFT*/
function approveDraft(e, editionId) {
    saveButtons.attr("disabled", true);
    approveButtons.html("<i class='fa fa-spinner fa-spin'></i> Approving Draft...").attr("disabled", true);
    rejectButtons.attr("disabled", true);
    $.ajax({
        url: e.target.href,
        type: "POST",
        dataType: "json",
        data: { "id": editionId },
        success: function (result) {
            if (result.success === true) {
                approveButtons.html("<i class='fa fa-check'></i> Approved");
                swal({
                    text: result.message,
                    allowOutsideClick: false,
                    showCloseButton: true,
                    showConfirmButton: false
                }).then(function() {
                        location.href = result.returnUrl;
                    },
                    function() {
                        location.href = result.returnUrl;
                    });
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                toastr.error(result.message);
                OnError_Approve();
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            toastr.error(xhr.statusText);
            OnError_Approve();
        }
    });
};

function OnError_Approve() {
    saveButtons.attr("disabled", false);
    approveButtons.html("Approve").attr("disabled", false);
    rejectButtons.attr("disabled", false);
}

/*REJECT DRAFT*/
function rejectDraft(e, editionId, reason) {
    saveButtons.attr("disabled", true);
    approveButtons.attr("disabled", true);
    rejectButtons.html("<i class='fa fa-spinner fa-spin'></i> Rejecting Draft...").attr("disabled", true);
    $.ajax({
        url: e.target.href,
        type: "POST",
        dataType: "json",
        data: { "editionId": editionId, "reason": reason },
        success: function (result) {
            if (result.success === true) {
                rejectButtons.html("<i class='fa fa-check'></i> Rejected");
                swal({
                    title: "Rejected!",
                    text: result.message,
                    type: "success",
                    allowOutsideClick: false
                }).then(function() {
                        location.href = result.returnUrl;
                    },
                    function() {
                        location.href = result.returnUrl;
                    });
            } else {
                swal({
                    title: "Error!",
                    text: result.message,
                    type: "error"
                });
                toastr.error(result.message);
                OnError_Reject();
            }
        },
        error: function (xhr, textStatus, error) {
            swal({
                title: "Error!",
                text: xhr.statusText,
                type: textStatus
            });
            toastr.error(xhr.statusText);
            OnError_Reject();
        }
    });
};

function OnError_Reject() {
    saveButtons.attr("disabled", false);
    approveButtons.attr("disabled", false);
    rejectButtons.html("Reject Draft").attr("disabled", false);
}

$(document).ready(function () {
    $(".maxlength").maxlength({
        alwaysShow: true
    });
});

$(document).on("click", "#copyButton",
    function () {
        copy("divEdition");
        toastr.success("Copied!", { timeOut: 5000 });
    });

function copy(elementId) {
    var aux = document.createElement("div");
    aux.setAttribute("contentEditable", true);
    aux.innerHTML = document.getElementById(elementId).innerHTML;
    aux.setAttribute("onfocus", "document.execCommand('selectAll',false,null)");
    document.body.appendChild(aux);
    aux.focus();
    document.execCommand("copy");
    document.body.removeChild(aux);
}