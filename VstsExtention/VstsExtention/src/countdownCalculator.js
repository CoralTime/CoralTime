"use strict";
// ---------------------------------------------------------------------
// <copyright file="countdownCalculator.ts">
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>
//    This is part of the Countdown widget
//    from the ALM Rangers. This file contains the
//    code to calculate the countdown delta.
// </summary>
// ---------------------------------------------------------------------
exports.__esModule = true;
var countdownResult = require("./CountdownResult");
var CountdownCalculator = /** @class */ (function () {
    function CountdownCalculator(from, to, roundNumber, workingdays) {
        if (workingdays === void 0) { workingdays = []; }
        this.from = from;
        this.to = to;
        this.roundNumber = roundNumber;
        this.workingdays = workingdays;
    }
    CountdownCalculator.prototype.getDifference = function () {
        var _this = this;
        if (!this.isValid()) {
            return new countdownResult.CountdownResult(0, countdownResult.Unit.Invalid, this.roundNumber);
        }
        var diff = function (unit) {
            return _this.to.diff(_this.from, countdownResult.Unit[unit].toLowerCase(), unit === countdownResult.Unit.Days);
        };
        var numberOfExcludedDays = 0;
        if (this.workingdays.length > 0) {
            numberOfExcludedDays = this.countExcluded();
        }
        var test = diff(countdownResult.Unit.Days);
        this.to.add(-numberOfExcludedDays, countdownResult.Unit[countdownResult.Unit.Days].toLowerCase());
        var numberOfDays = diff(countdownResult.Unit.Days);
        if (numberOfDays >= 1.0) {
            // round up to the nearest 10th of a day and remove extranneous fractional part
            return new countdownResult.CountdownResult(numberOfDays, countdownResult.Unit.Days, this.roundNumber);
        }
        else {
            var numberOfHours = diff(countdownResult.Unit.Hours);
            if (numberOfHours >= 1) {
                return new countdownResult.CountdownResult(numberOfHours, countdownResult.Unit.Hours, this.roundNumber);
            }
            else {
                var numberOfMinutes = diff(countdownResult.Unit.Minutes);
                if (numberOfMinutes >= 1) {
                    return new countdownResult.CountdownResult(numberOfMinutes, countdownResult.Unit.Minutes, this.roundNumber);
                }
                else {
                    var numberOfSeconds = diff(countdownResult.Unit.Seconds);
                    return new countdownResult.CountdownResult(numberOfSeconds, countdownResult.Unit.Seconds, this.roundNumber);
                }
            }
        }
    };
    CountdownCalculator.prototype.isValid = function () {
        return this.from.isBefore(this.to);
    };
    CountdownCalculator.prototype.getDayOfWeekNumber = function (day) {
        if (typeof day === "string") { // for compatibility with old API
            switch (day) {
                case "sunday": return 0;
                case "monday": return 1;
                case "tuesday": return 2;
                case "wednesday": return 3;
                case "thursday": return 4;
                case "friday": return 5;
                case "saturday": return 6;
            }
        }
        else {
            return day;
        }
    };
    CountdownCalculator.prototype.isWorkDay = function (day) {
        for (var _i = 0, _a = this.workingdays; _i < _a.length; _i++) {
            var dayString = _a[_i];
            var workingDay = this.getDayOfWeekNumber(dayString);
            if (workingDay === day) {
                return true;
            }
        }
        return false;
    };
    CountdownCalculator.prototype.countExcluded = function () {
        var days = this.to.diff(this.from, "days");
        var excludedDays = 0;
        for (var i = 1; i <= days; i++) { // starting from 1 for not include the current day
            var weekday = this.from.clone().add(i, "days").weekday();
            if (!this.isWorkDay(weekday)) {
                excludedDays++;
            }
        }
        return excludedDays;
    };
    return CountdownCalculator;
}());
exports.CountdownCalculator = CountdownCalculator;
