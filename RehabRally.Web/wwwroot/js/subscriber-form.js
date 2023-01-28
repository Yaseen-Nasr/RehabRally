$(document).ready(function () {
    $("#js-governorate").change(function () {
        var selectedVal = $("#js-governorate option:selected").val();
        var url = '/Subscribers/GetAreas?governarateId=' + selectedVal;
        $.post({
            url: url,
            success: function (areas) {
                $('#js-area').find('option:not(:first)').remove();
                $.each(areas, function (i, area) {
                    $('#js-area').append($('<option>', {
                        value: area.value,
                        text: area.text
                    }));
                });
            },
            error: function () {
                showErrorMessage();
            }
        });

    });
});