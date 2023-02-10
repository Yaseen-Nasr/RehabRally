
function onAddCopySuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    $('tbody').prepend(row);
    KTMenu.createInstances();

    var count = $('#ExercisesCount');
    console.log("count " + parseInt(count.text()));
    var newCount = parseInt(count.text()) + 1;
    console.log("fggfgffg " + newCount);
    count.text(newCount);

    $('.js-alert').addClass('d-none');
    $('table').removeClass('d-none');
}

function onEditCopySuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    $(updatedRow).replaceWith(row);
    KTMenu.createInstances();
}

function onChangeCategory() {

    var selectedVal = $("#js-category option:selected").val();
    var url = '/Users/GetCategoryExercises?categoryId=' + selectedVal;
    $.post({
        url: url,
        success: function (exercises) {

            $('#js-exercises').find('option:not(:first)').remove();
            $.each(exercises, function (i, exercise) {
                $('#js-exercises').append($('<option>', {
                    value: exercise.value,
                    text: exercise.text
                }));
            });
        },
        error: function () {
            showErrorMessage();
        }
    });

};