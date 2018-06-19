window.SkipDecriptionWhenFound = [];
window.ReplaceInDecription = [];
window.DecimalSeparator = "";


function AddWordToSkip(s) {
    console.log("List-Skip string:" + s);
    window.SkipDecriptionWhenFound.push(s);
    $("#ulSkip").append('<li class="list-group-item">' + s + '</li >');
    $('#wordSkipInput').val('');
}

function AddWordToRemove(s) {
    console.log("List-Remove string:" + s);
    window.ReplaceInDecription.push(s);
    $("#ulRemove").append('<li class="list-group-item">' + s + '</li >');
    $('#wordRemoveInput').val('');
}


function GetPDP() {

    var Domain = $('#domainInput').val();

    if (Domain == "") return null;
    console.log("decimal separator:" + window.DecimalSeparator);
    
    var CurrencySeparators = [];
    for (let i = 1; i < 5; i++) {
        let s = $("#checkBut" + i).val();
        console.log("separator:" + s);
        if (s != window.DecimalSeparator && $("#checkBut" + i).is(':checked')) {
            CurrencySeparators.push(s);
        }
    }
    CurrencySeparators.push(" ");
    CurrencySeparators.push("\t");
    CurrencySeparators.push("\r");
    CurrencySeparators.push("\n");
    CurrencySeparators.push("\n");

    return {
        Domain: Domain,
        SkipDecriptionWhenFound: window.SkipDecriptionWhenFound,
        ReplaceInDecription: window.ReplaceInDecription,
        CurrencySeparators: CurrencySeparators,
        DecimalSeparator: window.DecimalSeparator,
        DescriptionGetKind: $('input[name=descriptionMethod1]:radio:checked').val()*1,
        SearchPriceKind: $('input[name=searchMethod1]:radio:checked').val()*1
    };
}

$(function () {

    $(".dropdown-menu").on('click', 'li a', function () {
        var s = $(this).text();
        console.log("decimal separator; s=" + s);
        $(".btn:first-child").text(s);
        $(".btn:first-child").val(s);
        switch (s) {
            case "\.": window.DecimalSeparator = "\\."; break;
            case ",": window.DecimalSeparator = ","; break;
            default: window.DecimalSeparator = "";
        }
        console.log("decimal separator=" + window.DecimalSeparator); 
    });

});