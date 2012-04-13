$(document).ready(function () {
  $("input").placeholder();
  $("textarea").placeholder();

  $("#award").autocomplete({
    source: "/autosuggest/awards",
    minLength: 2
  });


  // useless, 255 kom je toch nooit aan
  //$("#award").bind("keydown", maxCharacters);

});