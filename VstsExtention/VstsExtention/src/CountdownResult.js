"use strict";
// ---------------------------------------------------------------------
// <copyright file="countdownResult.ts">
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>
//    This is part of the Countdown widget
//    from the ALM Rangers. This file contains the
//    code for a countdown result and unit.
// </summary>
// ---------------------------------------------------------------------
exports.__esModule = true;
var CountdownResult = /** @class */ (function () {
    function CountdownResult(value, unit, roundNumber) {
        this.value = value;
        this.unit = unit;
        this.roundNumber = roundNumber;
    }
    CountdownResult.prototype.getValueFontSize = function () {
        if (this.getDisplayValue().length >= 4) {
            return "55px";
        }
        return "72px";
    };
    CountdownResult.prototype.getDisplayValue = function () {
        if (!this.roundNumber && this.unit === Unit.Days) {
            return this.value.toFixed(1);
        }
        return this.value.toFixed(0);
    };
    CountdownResult.prototype.getDisplayUnit = function () {
        return Unit[this.unit].toLowerCase();
    };
    CountdownResult.prototype.isLessThan = function (threshold, unit) {
        if (this.unit < unit) {
            return true;
        }
        if (this.unit === unit) {
            return this.value < threshold;
        }
        return false;
    };
    return CountdownResult;
}());
exports.CountdownResult = CountdownResult;
var Unit;
(function (Unit) {
    Unit[Unit["Invalid"] = 0] = "Invalid";
    Unit[Unit["Seconds"] = 1] = "Seconds";
    Unit[Unit["Minutes"] = 2] = "Minutes";
    Unit[Unit["Hours"] = 3] = "Hours";
    Unit[Unit["Days"] = 4] = "Days";
    Unit[Unit["Years"] = 5] = "Years";
})(Unit = exports.Unit || (exports.Unit = {}));
