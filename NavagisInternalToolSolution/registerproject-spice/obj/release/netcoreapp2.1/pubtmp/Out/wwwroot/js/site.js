// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(document).ready(function () {
    $("#ButtonGoogle").click(function () {
        $.post("/Index?handler=AuthGoogle",
            {
                name: "Donald Duck",
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            function (data) {
                console.log(data);
                //window.location.replace(data);
                data = data.replace(/['"]+/g, '');
                window.location.href = data;
            }
        );
    });
});
$(document).ready(function () {
    $("#submitButton").click(function () {
        var checkbox_value = "";
        let hasSelectedAProject = false;
        $(":radio").each(function () {
            var ischecked = $(this).is(":checked");
            if (ischecked) {
                checkbox_value += $(this).val();
                hasSelectedAProject = true;
            }
        });

        if (hasSelectedAProject == false)
        {
            alert("Please select a project.");
            return false;
        }

        $(".loading-icon").removeClass("hide");
        $(".button").attr("disabled", true);
        $(".btn-txt").text("Processing...");

        $.post("/Index?handler=ChangeBilling",
            {
                resource: checkbox_value,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            function (data) {
                data = data.replace(/['"]+/g, '');
                
                const cardErrors = document.getElementById('card-errors');
                if (data == "BAI_ERROR") {
                    cardErrors.textContent = "No BAI Specified. Please consult Navagis or use another email.";
                    cardErrors.classList.add('visible');
                }
                else if (data == "BAI_PERMISSION_ERROR") {
                    window.location.href = "/CustomMessages/Error";
                }
                else if (data == "BAI_NEED_TO_BE_CONFIGURED") {
                    window.location.href = "/CustomMessages/ErrorBAI";
                }
                else {
                    window.location.href = "/CustomMessages/Success";
                }
            }
        );
    });
});