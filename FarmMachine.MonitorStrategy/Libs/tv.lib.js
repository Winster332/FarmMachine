function getButtonBacktesting() {
    var divs = document.getElementsByTagName('div');
    var targetButton = null;

    for (var i = 0; i < divs.length; i++) {
        var element = divs[i]; var dataName = element.getAttribute('data-name');

        if (dataName != null && dataName != undefined && dataName == 'backtesting') {
            targetButton = element;
        }
    }

    return targetButton;
}

/// 0 - browse, 1 - show detail indicator, 2 - list orders
function getButtonBacktestListOrders(number) {
    var wrapperDiv = document.getElementsByClassName('backtesting-select-wrapper').item(0);
    var button = wrapperDiv.children.item(0).children.item(number);

    return button;
}

function backtestListOrderScrollToBottom() {
    var needValue = document.getElementsByClassName('report-content trades')[0].scrollHeight;
    document.getElementsByClassName('report-content trades')[0].scrollTop = needValue;
}

function openLastBacktestOrderList() {
    var backtestBtn = getButtonBacktesting();

    if (backtestBtn.getAttribute('data-active') === 'false') {
        backtestBtn.click();
    }

    // getButtonBacktestListOrders(2).click();
    // backtestListOrderScrollToBottom();
}