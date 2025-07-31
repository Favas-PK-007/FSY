$('[data-rel="tooltip"]').each(function () {
  new bootstrap.Tooltip(this);
});


// treeview

$(document).ready(function () {
  $(".tree-view ul").each(function () {
    const $ul = $(this);
    const $li = $ul.parent();

    if ($li.children(".indicator").length === 0) {
      const $span = $(
        '<span class="indicator"><i class="fa-solid fa-chevron-right"></i></span>'
      );
      $li.prepend($span);
    }

    $ul.addClass("collapse");
  });

  $(".tree-view").on("click", function (e) {
    const $target = $(e.target);
    const $li = $target.closest("li");

    if (!$li.length) return;

    const isIndicator = $target.closest(".indicator").length > 0;
    const isLabel = $target.is("label");

    if (!isIndicator && !isLabel) return;

    const $childUl = $li.children("ul").first();
    const $icon = $li.children(".indicator").children("svg");

    if (!$childUl.length || !$icon.length) return;

    if ($childUl.hasClass("collapse")) {
      $childUl.removeClass("collapse").addClass("expand");
      $icon.removeClass("fa-chevron-right").addClass("fa-chevron-down");
    } else {
      $childUl.removeClass("expand").addClass("collapse");
      $icon.removeClass("fa-chevron-down").addClass("fa-chevron-right");
    }
  });
});