var clickAttr;

//CALLING METHOD IN THE CONTROLLER TO DO SOME WORK
function StartProcess(caller, url) {

    clickAttr = $(caller).attr("onclick");

    $(caller).attr("onclick", "void(0); alert('click not allowed');");
    $(caller).html("<small><i class='fa fa-pause fa-2x text-success'></i></small>");

    var taskId = $(caller).data("id");
    var progressresult = $("#progressresult" + taskId);
    progressresult.text("0%").width(0);

    var connectionId = $.connection.hub.id;
    url = url + "&connectionId=" + connectionId;

    $.getJSON(url,
        {},
        function (data) {
            if (!data) {
                //toastr.success("Completed")
            }
            else {
                alert(data);
            }
        });
}

$(function () {
    // Reference the auto-generated proxy for the hub.
    var progress = $.connection.progressHub;

    // Create a function that the hub can call back to display messages.
    progress.client.AddProgress = function (message, percentage, taskId) {

        //ProgressBarModal("show", message + " " + percentage);
        //$('#ProgressMessage').width(percentage);

        var progressouter = $("#progressouter" + taskId);
        var progressresult = $("#progressresult" + taskId);
        var runbutton = $("#runbutton" + taskId);

        var progressmessage = $("#progressmessage" + taskId);
        progressmessage.html(message);

        runbutton.hide();
        progressouter.show();
        progressresult.text(percentage).width(percentage);

        if (percentage == "100%") {
            //ProgressBarModal();

            progressouter.hide("slow", function () {
                runbutton.show("slow");
            });

            runbutton.attr("onclick", clickAttr);
            runbutton.html("<small><i class='fa fa-play fa-2x text-success'></i></small>");

            progressmessage.fadeIn().text("DONE");
            setTimeout(function () {
                progressmessage.fadeOut()
            }, 10000);
        }
    };

    $.connection.hub.start().done(function () {
        var connectionId = $.connection.hub.id;
    });
});