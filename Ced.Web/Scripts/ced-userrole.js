var btnSearch = $("#btnSearch");

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
    $("#searchUserRoleForm").submit(function (e) {
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

function refreshUserRoles(userEmail, roleId) {
    if (userEmail === null || userEmail === undefined) {
        userEmail = $("#emailSearchBox").val();
    }
    var roleIds = $("input:checkbox[name='RoleId']:checked").map(function () {
        return $(this).val();
    }).get();
    if (userEmail === "") {
        toastr.warning("You must select at least one search option.");
        btnSearch.html("Search").attr("disabled", false);
        return;
    }

    $.ajax({
        url: userRoleSearchUrl,
        type: "POST",
        data: { UserEmail: userEmail, RoleId: roleIds },
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

$("body").on("click", ".del-userrole", function (e) {
    var delBtn = $(e.currentTarget);
    e.preventDefault();

    swal({
        title: "Are you sure?",
        text: "You are about to delete this user role.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!"
    }).then(function () {
        var innerSpan = $(delBtn.get(0).firstElementChild);
        var userRoleId = delBtn.attr("data-id");
        delBtn.addClass("disabled");
        innerSpan.addClass("fa-spin");

        $.ajax({
            url: userRoleDelUrl,
            data: { "id": userRoleId },
            dataType: "json",
            success: function (result) {
                if (result.success === true) {
                    swal({
                        title: "Deleted!",
                        text: result.message,
                        type: "success"
                    });
                    refreshUserRoles();
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
    $("#addUserRoleForm").submit(function (e) {
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
                        title: "Added!",
                        text: result.message,
                        type: "success",
                        allowOutsideClick: false
                    }).then(function () {
                            refreshUserRoles();
                    },
                        function () {
                            refreshUserRoles();
                        });
                } else {
                    swal({
                        title: "Error!",
                        text: result.message,
                        type: "error"
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

$("#appdropdown").change(function () {
    var appId = this.value;
    if (appId !== '')
        $("#rolesDiv").show();
    else
        $("#rolesDiv").hide();
    setRolesByApp(appId);
});

function setRolesByApp(appId) {
    $("#roledropdown > option").hide();
    $("#roledropdown > option[data-appid=" + appId + "]").show();
}

$(document).ready(function () {
    hideAllDropdowns();
});

$("#roledropdown").change(function () {
    hideAllDropdowns();
    var roleId = this.value;
    if (roleId == '4' || roleId == '12') // Industry Director
        $("#divIndustry").show();
    else if (roleId == '5' || roleId == '13') // Region Director
        $("#divRegion").show();
    else if (roleId == '6' || roleId == '14') // Country Director
        $("#divCountry").show();
});

function hideAllDropdowns() {
    $("#divRegion").hide();
    $("#divCountry").hide();
    $("#divIndustry").hide();
}

$('input[type="checkbox"][name="' + appCheckboxName + '"]').change(function () {
    var checkedAppCount = $('input[type="checkbox"][name="' + appCheckboxName + '"]:checked').length;
    if (checkedAppCount > 1 || checkedAppCount === 0) {
        $("#roles").hide();
    } else {
        $("#roles").show();
        $("div.roleCbDiv").hide();
        var appId = getCheckedAppId();
        $("div.roleCbDiv[data-appid=" + appId + "]").show();
    }
});

function getCheckedAppId() {
    var id;
    $('input[type="checkbox"][name="' + appCheckboxName + '"]:checked').each(function () {
        var checkedAppCount = $('input[type="checkbox"][name="' + appCheckboxName + '"]:checked').length;
        if (checkedAppCount === 1) {
            if (this.checked) {
                id = $(this).val();
            }
        }
    });
    return id;
}