﻿var ComponentTypeSelector = function (control, callback) {
    var me = this;
    me.control = control;
    me.callback = callback;
    me.SetValue(control.val());
    me.control.on('change', function () { me.onChange(); });
}

ComponentTypeSelector.prototype.SetValue = function(value) {
    this.control.data('paramvalue', value);
}

ComponentTypeSelector.prototype.onChange = function() {
    this.SetValue(this.control.val());
    if (this.callback) {
        this.callback();
    }
}