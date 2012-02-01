﻿/*!
* jQuery.parseJSON() extension (supports ISO & Asp.net date conversion)
*
* Version 1.0 (13 Jan 2011)
*
* Copyright (c) 2011 Robert Koritnik,
* Licensed under the terms of the MIT license
* http://www.opensource.org/licenses/mit-license.php
*/
(function ($) {

	// JSON RegExp
	var rvalidchars = /^[\],:{}\s]*$/;
	var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
	var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
	var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
	var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
	var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

	// replacer RegExp
	var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
	var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

	// determine JSON native support
	var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
	var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

	var jsonDateConverter = function (key, value) {
		if (typeof (value) === "string") {
			if (dateISO.test(value)) {
				return new Date(value);
			}
			if (dateNet.test(value)) {
				return new Date(parseInt(dateNet.exec(value)[1], 10));
			}
		}
		return value;
	};

	$.extend({
		parseJSON: function (data, convertDates) {
			/// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
			/// <param name="data" type="String">The JSON string to parse.</param>
			/// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>

			// convertDates = convertDates === false ? false : true;

			if (typeof data !== "string" || !data) {
				return null;
			}

			// Make sure leading/trailing whitespace is removed (IE can't handle it)
			data = $.trim(data);

			// Make sure the incoming data is actual JSON
			// Logic borrowed from http://json.org/json2.js
			if (rvalidchars.test(data
				.replace(rvalidescape, "@")
				.replace(rvalidtokens, "]")
				.replace(rvalidbraces, ""))) {
				// Try to use the native JSON parser
				if (extendedJSON || (nativeJSON && convertDates !== true)) {
					return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
				}
				else {
					data = convertDates === true ?
						data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
							.replace(replaceNet, "new Date($1)") :
						data;
					return (new Function("return " + data))();
				}
			} else {
				$.error("Invalid JSON: " + data);
			}
		}
	});



})(jQuery);

//jqAuto -- main binding (should contain additional options to pass to autocomplete)
//jqAutoSource -- the array to populate with choices (needs to be an observableArray)
//jqAutoQuery -- function to return choices
//jqAutoValue -- where to write the selected value
//jqAutoSourceLabel -- the property that should be displayed in the possible choices
//jqAutoSourceInputValue -- the property that should be displayed in the input box
//jqAutoSourceValue -- the property to use for the value
ko.bindingHandlers.jqAuto = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var options = valueAccessor() || {},
            allBindings = allBindingsAccessor(),
            unwrap = ko.utils.unwrapObservable,
            modelValue = allBindings.jqAutoValue,
            source = allBindings.jqAutoSource,
            query = allBindings.jqAutoQuery,
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp,
            labelProp = allBindings.jqAutoSourceLabel || inputValueProp;

        //function that is shared by both select and change event handlers
        function writeValueToModel(valueToWrite) {
            if (ko.isWriteableObservable(modelValue)) {
                modelValue(valueToWrite);
            } else {  //write to non-observable
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['jqAutoValue'])
                    allBindings['_ko_property_writers']['jqAutoValue'](valueToWrite);
            }
        }

        //on a selection write the proper value to the model
        options.select = function (event, ui) {
            writeValueToModel(ui.item ? ui.item.actualValue : null);
        };

        //on a change, make sure that it is a valid value or clear out the model value
        options.change = function (event, ui) {
            var currentValue = $(element).val();
            var matchingItem = ko.utils.arrayFirst(unwrap(source), function (item) {
                return unwrap(inputValueProp ? item[inputValueProp] : item) === currentValue;
            });

            if (!matchingItem) {
                writeValueToModel(null);
            }
        }

        //hold the autocomplete current response
        var currentResponse = null;

        //handle the choices being updated in a DO, to decouple value updates from source (options) updates
        var mappedSource = ko.dependentObservable({
            read: function () {
                mapped = ko.utils.arrayMap(unwrap(source), function (item) {
                    var result = {};
                    result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
                    result.value = inputValueProp ? unwrap(item[inputValueProp]) : unwrap(item).toString();  //show in input box
                    result.actualValue = valueProp ? unwrap(item[valueProp]) : item;  //store in model
                    return result;
                });
                return mapped;
            },
            write: function (newValue) {
                source(newValue);  //update the source observableArray, so our mapped value (above) is correct
                if (currentResponse) {
                    currentResponse(mappedSource());
                }
            },
            disposeWhenNodeIsRemoved: element
        });

        if (query) {
            options.source = function (request, response) {
                currentResponse = response;
                query.call(this, request.term, mappedSource);
            }
        } else {
            //whenever the items that make up the source are updated, make sure that autocomplete knows it
            mappedSource.subscribe(function (newValue) {
                $(element).autocomplete("option", "source", newValue);
            });

            options.source = mappedSource();
        }


        //initialize autocomplete
        $(element).autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var allBindings = allBindingsAccessor(),
           unwrap = ko.utils.unwrapObservable,
           modelValue = unwrap(allBindings.jqAutoValue) || '',
           valueProp = allBindings.jqAutoSourceValue,
           inputValueProp = allBindings.jqAutoSourceInputValue || valueProp;

        //if we are writing a different property to the input than we are writing to the model, then locate the object
        if (valueProp && inputValueProp !== valueProp) {
            var source = unwrap(allBindings.jqAutoSource) || [];
            var modelValue = ko.utils.arrayFirst(source, function (item) {
                return unwrap(item[valueProp]) === modelValue;
            }) || {};
        }

        //update the element with the value that should be shown in the input
        $(element).val(modelValue && inputValueProp !== valueProp ? unwrap(modelValue[inputValueProp]) : modelValue.toString());
    }
};

ko.bindingHandlers.autoComplete = {
    findSelectedItem: function (dataSource, binding, selectedValue) {
        var unwrap = ko.utils.unwrapObservable;
        //Go through the source and find the id, and use its label to set the autocomplete
        var source = unwrap(dataSource);
        var valueProp = unwrap(binding.optionsValue);

        var selectedItem = ko.utils.arrayFirst(source, function (item) {
            if (unwrap(item[valueProp]) === selectedValue)
                return true;
        }, this);

        return selectedItem;
    },
    buildDataSource: function (dataSource, labelProp, valueProp) {
        var unwrap = ko.utils.unwrapObservable;
        var source = unwrap(dataSource);
        var mapped = ko.utils.arrayMap(source, function (item) {
            var result = {};
            result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
            result.value = valueProp ? unwrap(item[valueProp]) : unwrap(item).toString();  //value 
            return result;
        });
        return mapped;
    },
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var valueProp = unwrap(binding.optionsValue);
        var labelProp = unwrap(binding.optionsText) || valueProp;
        var displayId = $(element).attr('id') + '-display';
        var displayElement;
        var options = {};

        if (binding.autoCompleteOptions) {
            options = $.extend(options, binding.autoCompleteOptions);
        }

        //Create a new input to be the autocomplete so that the label shows
        // also hide the original control since it will be used for the value binding
        $(element).hide();
        $(element).after('<input type="text" id="' + displayId + '" />')
        displayElement = $('#' + displayId);

        //handle value changing
        var modelValue = binding.value;
        if (modelValue) {
            var handleValueChange = function (event, ui) {
                var labelToWrite = ui.item ? ui.item.label : null
                var valueToWrite = ui.item ? ui.item.value : null;
                //The Label and Value should not be null, if it is
                // then they did not make a selection so do not update the 
                // ko model
                if (labelToWrite && valueToWrite) {
                    if (ko.isWriteableObservable(modelValue)) {
                        //Since this is an observable, the update part will fire and select the 
                        //  appropriate display values in the controls
                        modelValue(valueToWrite);
                    } else {  //write to non-observable
                        if (binding['_ko_property_writers'] && binding['_ko_property_writers']['value']) {
                            binding['_ko_property_writers']['value'](valueToWrite);
                            //Because this is not an observable, we have to manually change the controls values
                            // since update will not do it for us (it will not fire since it is not observable)
                            displayElement.val(labelToWrite);
                            $(element).val(valueToWrite);
                        }
                    }
                }
                //They did not make a valid selection so change the autoComplete box back to the previous selection
                else {
                    var currentModelValue = unwrap(modelValue);
                    //If the currentModelValue exists and is not nothing, then find out the display
                    // otherwise just blank it out since it is an invalid value
                    if (!currentModelValue)
                        displayElement.val('');
                    else {
                        //Go through the source and find the id, and use its label to set the autocomplete
                        var selectedItem = ko.bindingHandlers.autoComplete.findSelectedItem(dataSource, binding, currentModelValue);

                        //If we found the item then update the display
                        if (selectedItem) {
                            var displayText = labelProp ? unwrap(selectedItem[labelProp]) : unwrap(selectedItem).toString();
                            displayElement.val(displayText);
                        }
                        //if we did not find the item, then just blank it out, because it is an invalid value
                        else {
                            displayElement.val('');
                        }
                    }
                }

                return false;
            };

            var handleFocus = function (event, ui) {
                $(displayElement).val(ui.item.label);
                return false;
            };

            options.change = handleValueChange;
            options.select = handleValueChange;
            options.focus = handleFocus;
            //options.close = handleValueChange;
        }

        //handle the choices being updated in a Dependant Observable (DO), so the update function doesn't 
        // have to do it each time the value is updated. Since we are passing the dataSource in DO, if it is
        // an observable, when you change the dataSource, the dependentObservable will be re-evaluated
        // and its subscribe event will fire allowing us to update the autocomplete datasource
        var mappedSource = ko.dependentObservable(function () {
            return ko.bindingHandlers.autoComplete.buildDataSource(dataSource, labelProp, valueProp);
        }, viewModel);
        //Subscribe to the knockout observable array to get new/remove items
        mappedSource.subscribe(function (newValue) {
            displayElement.autocomplete("option", "source", newValue);
        });

        options.source = mappedSource();

        displayElement.autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var valueProp = unwrap(binding.optionsValue);
        var labelProp = unwrap(binding.optionsText) || valueProp;
        var displayId = $(element).attr('id') + '-display';
        var displayElement = $('#' + displayId);
        var modelValue = binding.value;

        if (modelValue) {
            var currentModelValue = unwrap(modelValue);
            //Set the hidden box to be the same as the viewModels Bound property
            $(element).val(currentModelValue);
            //Go through the source and find the id, and use its label to set the autocomplete
            var selectedItem = ko.bindingHandlers.autoComplete.findSelectedItem(dataSource, binding, currentModelValue);
            if (selectedItem) {
                var displayText = labelProp ? unwrap(selectedItem[labelProp]) : unwrap(selectedItem).toString();
                displayElement.val(displayText);
            }
        }
    }
};

function split(val) {
	return val.split(/,\s*/);
}
function extractLast(term) {
	return split(term).pop();
}

$(document).ready(function () {
    $('#ToggleAddNewBoxesButton').click(function () {
        $('.addItemFromStubDiv').toggle('highlight', {}, 400);
        // return false;
    });

    $("#saveButton")
			.button()
    //			.click(function () {
    //			    alert("Running the last action");
    //			})
			.next()
				.button({
				    text: false,
				    icons: {
				        primary: "ui-icon-triangle-1-s"
				    }
				})
				.click(function () {
				    var menu = $(this).parent().next().show().position({
				        my: "left top",
				        at: "left bottom",
				        of: this
				    });
				    $(document).one("click", function () {
				        menu.hide();
				    });
				    return false;
				})
				.parent()
					.buttonset()
					.next()
						.hide()
						.menu();

    $("#QuickAddProjectNameText")
    // don't navigate away from the field on tab when selecting an item
			.bind("keydown", function (event) {
			    if (event.keyCode === $.ui.keyCode.TAB &&
						$(this).data("autocomplete").menu.active) {
			        event.preventDefault();
			    }
			})
			.autocomplete({
			    minLength: 0,
			    source: function (request, response) {
			        // delegate back to autocomplete, but extract the last term
			        response($.ui.autocomplete.filter(statusReportVM.Report().ProjectNames(),
						request.term));

			        //extractLast(request.term)));
			    },
			    select: function (event, ui) {
			        // this.value = ui.item.value;
			        // update the binding directly?
			        statusReportVM.Report().QuickAddProjectName(ui.item.value);
			        //console.log(this.value);
			    },
			    change: function (event, ui) {
			        // this.value = ui.item.value;
			        // update the binding directly?
			        statusReportVM.Report().QuickAddProjectName(ui.item.value);
			        //console.log(this.value);
			    }
			})
		;
    $('#QuickAddCaptionText').focus();

    $('#QuickAddMilestoneDateText').change(function () {
        // figure out the new milestone date
        var milestoneDate = $('#QuickAddMilestoneDateText').val();
        var statusReportDate = statusReportVM.Report().PeriodStart();
        var milestoneType = rollMilestone(statusReportDate, milestoneDate);
        $('#QuickAddMilestoneTypes').val(milestoneType);
    });

    
//    $('#NewStatusItemMilestoneDateText').change(function () {
//        // figure out the new milestone date
//        var milestoneDate = $('#NewStatusItemMilestoneDateText').val();
//        var statusReportDate = statusReportVM.Report().PeriodStart();
//        var milestoneType = rollMilestone(statusReportDate, milestoneDate);
//        $('#NewStatusItemMilestoneTypes').val(milestoneType);
//    });

    // bind 's' key to save button click (for some reason, save function direct call shows no itemstoremove)
    $(document).bind('keydown', 's', function () { $('#saveButton').click(); });
});

var rollMilestone = function (reportDate, itemDate) {
    var statusItemDate = new Date(itemDate);
    var statusReportDate = new Date(reportDate);
    var minDate = new Date(reportDate);
    var maxDate = new Date(reportDate);
    minDate.add(-7).days();
    maxDate.add(7).days();

    if (statusItemDate < minDate)
        return 2;
    else if (statusItemDate < statusReportDate)
        return 0;
    else if (statusItemDate >= statusReportDate && statusItemDate < maxDate)
        return 1;

    return 3; // always OpenItem for now as test
    /*
    0=LastWeek
    1=ThisWeek
    2=OpenItem
    3=Milestone
    if (sourceStatusItem.Milestone.Date < statusReportDate.AddDays(-7))
    return null;
    if (sourceStatusItem.Milestone.Date < statusReportDate)
    si.Milestone.Type = MilestoneTypes.LastWeek;
    else if (sourceStatusItem.Milestone.Date >= statusReportDate &&
    sourceStatusItem.Milestone.Date < statusReportDate.AddDays(7))
    si.Milestone.Type = MilestoneTypes.ThisWeek;*/
};
 
var jsonDateRE = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;

var parseJsonDateString = function (value) {
	var arr = value && jsonDateRE.exec(value);
	if (arr) {
		return new Date(parseInt(arr[1]));
	}
	return value;
};

var getShortDate = function (dateValue) {
	var val = ko.utils.unwrapObservable(dateValue);
	if (val) {
		var d = parseJsonDateString(val);
		var dt = new Date(d);
		return dt.toString("MM-dd-yyyy");
	}
	return val;
};

ko.bindingHandlers.datepicker = {
	init: function (element, valueAccessor, allBindingsAccessor) {
		//initialize datepicker with some optional options
		var options = allBindingsAccessor().datepickerOptions || {};
		$(element).datepicker(options);

		//handle the field changing
		ko.utils.registerEventHandler(element, "change", function () {
			var observable = valueAccessor();
			observable($(element).datepicker("getDate"));
		});

		//handle disposal (if KO removes by the template binding)
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$(element).datepicker("destroy");
		});

	},
	update: function (element, valueAccessor) {
		var value = ko.utils.unwrapObservable(valueAccessor());
		//console.log("value", value);
		$(element).datepicker("setDate", value);
	}
};


var statusReportVM = {
    Report: ko.observable(new statusReport()),
    initJQuery: function () {
        $("#tabs").tabs({ spinner: 'Retrieving data...' });
        // $(".datefield").datepicker({dateFormat:'yy-mm-dd',changeMonth:true,changeYear:true });
        $('.statusCaptionText').autoResize({

        });
        //	    {
        //			// On resize:
        //			onResize: function () {
        //				$(this).css({ opacity: 0.8 });
        //			},
        //			// After resize:
        //			animateCallback: function () {
        //				$(this).css({ opacity: 1 });
        //			},
        //			// Quite slow animation:
        //			animateDuration: 300,
        //			// More extra space:
        //			extraSpace: 40
        //		});
        $('#statusDateSelect').change(function () {
            // alert('selected');
            $('#ChangeStatusDateForm').submit();
        });
        $('.statusTags').tagsInput({
            'defaultText': 'add a tag',
            'placeholderColor': '#666666'
        });

    },
    loadReport: function (reportDate) {
        var url = "/StatusReport/GetStatusReport?statusDate=" + reportDate;
        $.ajax({
            url: url,
            dataType: "json",
            converters: {
                "text json": function (data) {
                    return $.parseJSON(data, true);
                }
            },
            success: function (response) {
                if (response != null) {
                    var sr = new statusReport()
						.initData(response);
                    statusReportVM.Report(sr);
                    statusReportVM.initJQuery();

                } else {
                    alert(response.message);
                }
            },
            error: function (response) {
                alert("Failed to get report for date " + reportDate + "..." + response.responseText);
            }
        });
    }
};

RedirectToReport = function (statusDate) {
	// alert(statusDate);
	//window.location.replace("?statusDate=" + this.PeriodStart());
};

function statusReport() {
	var self = this;
	this.Id = ko.observable(0);
	this.PeriodStart = ko.observable(null);
	
	this.Caption = ko.observable('');
	this.NumberOfStatusItems = ko.observable(0);
	this.Items = ko.observableArray([]);

	this.StatusReportDates = ko.observableArray([]);
	
	this.StatusItemToAdd = ko.observable('');
	this.StatusItemDateToAdd = ko.observable(new Date());
	this.StatusItemMilestoneToAdd = ko.observable(0);

	this.ItemsByProject = ko.observableArray([]);
	this.ItemsByTeam = ko.observableArray([]);
	this.ItemsToRemove = ko.observableArray([]);
	this.CanRollStatus = ko.observable(false);
	this.RollStatusDate = ko.observable(null);

	this.Projects = ko.observableArray([]);
	this.ProjectsAC = ko.observableArray([]);

	this.ProjectNames = ko.computed(function () {
		var arr = new Array();
		ko.utils.arrayForEach(self.Projects(), function (item) {
			arr.push(item.Name);
		});
		return arr;
	} .bind(this));
	
	// quickadd
	this.QuickAddCaption = ko.observable(null);
	this.QuickAddProjectName = ko.observable(null);
	this.QuickAddMilestoneDate = ko.observable(new Date());
	this.QuickAddMilestoneType = ko.observable(1);

	this.RollStatusDateFormatted = ko.computed(function () {
		if (self.RollStatusDate() != null) {
			var d = parseJsonDateString(self.RollStatusDate());
			var dt = new Date(d);
			return dt.toString("MM-dd-yyyy");
		}
		return null;
	} .bind(this));
	this.initData = function (response) {

		self.Caption(response.PeriodStart)
			.PeriodStart(response.PeriodStart)
			.Id(response.Id)
			.NumberOfStatusItems(response.NumberOfStatusItems)
			.StatusItemToAdd("")
			.StatusItemDateToAdd(new Date())
			.StatusItemMilestoneToAdd(0)
			.StatusReportDates(response.StatusReportDates)
			.CanRollStatus(response.CanRollStatus)
			.RollStatusDate(response.RollStatusDate)
			.Projects(response.Projects);
		$.each(response.Items, function (x, item) {
			var sri = new statusReportItem()
				.LoadFromObject(item);

			self.loadStatusItem(sri);
		});
		return self;
	};

	this.loadStatusItem = function (statusItem) {
		// autocreates team and project if not found
		// console.log("loadStatusItem called: " + statusItem.Id() + " / " + statusItem.Caption());
		var matchingItem = ko.utils.arrayFilter(self.Items(), function (item) {
			return item.Id() == statusItem.Id();
		});
		if (!matchingItem)
			self.Items.push(statusItem);
		else {
			//console.log("Matching item found for " + statusItem.Id());
		}
		var team = self.getOrCreateTeamFromStatusItem(statusItem);
	};

	this.PendingChangesCount = ko.computed(function () {
		var arrCount = ko.utils.arrayFilter(self.Items(), function (item) {
			return item.HasChanges();
		});
		
		// debugging
		ko.utils.arrayForEach(self.Items(), function(item) {
			if (item.HasChanges())
				console.log(ko.toJSON(item));
		});

		return arrCount.length;
	} .bind(this));
	
	this.PendingDeletionsCount = ko.computed(function() {
		return self.ItemsToRemove().length;
	} .bind(this));

	this.LogItems = function() {
		// debugging
//        ko.utils.arrayForEach(self.Items(), function(item) {
//            console.log(ko.toJSON(item)); // item.Id() + " / " + item.Caption());
//        });
	};

	this.reset = function () {
	    // remove item from project
	    ko.utils.arrayForEach(self.ItemsToRemove(), function (statusItemToRemove) {
	        var projectWithItem = ko.utils.arrayFirst(self.ItemsByProject(), function (item) {
	            return (item.ProjectId() == statusItemToRemove.ProjectId());
	        });
	        projectWithItem._destroyStatusItem(statusItemToRemove);
	    });
	    ko.utils.arrayForEach(self.Items(), function (item) {
	        item.reset();
	    });
	    self.ItemsToRemove.removeAll();
	};
	
	
	this.rollStatus = function () {
		$("#dialog-confirm").dialog({
			resizable: false,
			height: 280,
			modal: true,
			buttons: {
				"Roll": function () {
					var url = "/StatusReport/RollStatus";
					var miniReport = new statusReport();
					miniReport.Id = self.Id();
					console.log("About to roll status for " + miniReport.Id);

					$.ajax({
						url: url,
						dataType: "json",
						data: ko.toJSON({ report: miniReport }),
						converters: {
							"text json": function (data) {
								return $.parseJSON(data, true);
							}
						},
						type: "post",
						contentType: "application/json",
						success: function (response) {
							if (response != null) {
								var sr = new statusReport()
									.initData(response);
								statusReportVM.Report(sr);
								statusReportVM.initJQuery();

							} else {
								alert(response.message);
							}
						},
						error: function (xhr, status) {
							switch (status) {
								case 404:
									alert('File not found');
									break;
								case 500:
									alert('Server error');
									break;
								case 0:
									alert('Request aborted');
									break;
								default:
									alert('Unknown error ' + status);
							}
						}
					});
					$(this).dialog("close");
				},
				Cancel: function () {
					$(this).dialog("close");
				}
			}
		});
	
	};
	this.save = function () {
	    var url = "/StatusReport/Save";
	    var miniReport = new statusReport();
	    miniReport.PeriodStart = self.PeriodStart();
	    miniReport.Id = self.Id();
	    miniReport.Caption = self.Caption();

	    miniReport.Items = ko.utils.arrayFilter(self.Items(), function (item) {
	        return item.HasChanges() || item.HasInsertion(); // ko.utils.stringStartsWith(item.name().toLowerCase(), filter);
	    });
	    miniReport.ItemsToRemove = self.ItemsToRemove();
	    if (miniReport.Items.length == 0 && miniReport.ItemsToRemove.length == 0) {
	        console.log("No items to change, aborting save.");
	        return;
	    }
	    console.log("About to save self.Items = " + miniReport.Items.length);

	    $.ajax({
	        url: url,
	        dataType: "json",
	        converters: {
	            "text json": function (data) {
	                return $.parseJSON(data, true);
	            }
	        },
	        data: ko.toJSON({ report: miniReport }),
	        type: "post",
	        contentType: "application/json",
	        success: function (result) {
	            // alert(result);
	            // loop through the returned values and update items
	            var counter = 0;
	            ko.utils.arrayForEach(ko.utils.unwrapObservable(miniReport.Items), function (item) {
	                var resultItem = result.Items[counter];
	                item.LoadFromObject(resultItem);
	                counter++;
	            });
	            self.reset();
	        },
	        error: function (xhr, status) {
	            switch (status) {
	                case 404:
	                    alert('File not found');
	                    break;
	                case 500:
	                    alert('Server error');
	                    break;
	                case 0:
	                    alert('Request aborted');
	                    break;
	                default:
	                    alert('Unknown error ' + status);
	            }
	        }
	    });
	};
	
	

	this.teamCounter = 1;

	this.getOrCreateTeamFromStatusItem = function (statusItem) {
		// ItemsByTeam
		var teams = ($.grep(this.ItemsByTeam(), function (i) {
			return (i.Name() == statusItem.ProjectLeadFullName());
		}));
		var team = null;
		if (teams.length > 0) {
			team = teams[0];
		} else {
			team = new teamStatus()
				.Report(this)
				.TeamId(this.teamCounter++)
				.Name(statusItem.ProjectLeadFullName());
			// console.log("Created team " + team.Name());
			this.ItemsByTeam.push(team);
			//statusItem.ProjectTeamId()
		}

		team.addProject(this.getOrCreateProjectFromStatusItem(statusItem));
		return team;
	};

	this.getOrCreateProjectFromStatusItem = function (statusItem) {
		//ItemsByProject
		var projects = ($.grep(this.ItemsByProject(), function (i) {
			return (i.ProjectId() == statusItem.ProjectId());
		}));
		var proj = null;
		if (projects.length > 0) {
			proj = projects[0];
		} else {
			proj = new projectStatus()
				.Report(this)
				.ProjectId(statusItem.ProjectId())
				.ProjectName(statusItem.ProjectName())
				.ProjectDepartmentName(statusItem.ProjectDepartmentName())
				.ProjectDepartmentManagerFullName(statusItem.ProjectDepartmentManagerFullName())
				.ProjectType(statusItem.ProjectType())
				.ProjectTeamId(statusItem.ProjectTeamId())
				.ProjectTeamName(statusItem.ProjectTeamName())
				.ProjectLeadFullName(statusItem.ProjectLeadFullName())
				.ProjectTeamLeadFullName(statusItem.ProjectTeamLeadFullName());
			// console.log("Created project " + proj.ProjectName() + " - " + proj.ProjectId());
			this.ItemsByProject.push(proj);
		}
		proj.addItem(statusItem);
		return proj;
	};

	this.SelectedStatusReport = ko.observable(null);

	this.SelectedStatusReportFormatted = ko.computed(function () {
		return getShortDate(self.SelectedStatusReport);
	} .bind(self));
	
	this.PeriodStartFormatted = ko.computed(function () {
		return getShortDate(self.PeriodStart);
		if (self.PeriodStart()) {
			var d = parseJsonDateString(self.PeriodStart());
			var dt = new Date(d);
			return dt.toString("MM-dd-yyyy");
		}
		return self.PeriodStart();
	} .bind(self));

	this.Name = ko.computed(function() {
		return self.Caption() + " (" + self.PeriodStartFormatted() + ")";
	}.bind(this));
	
	this.addReport = function () {
		$.ajax({
			url: "/Home/Create/",
			type: 'post',
			data: ko.toJSON(this),
			contentType: 'application/json',
			success: function (result) {
				alert(result);
			}
		});
	};

	this.addStatusItem = function (sri) {
		this.Items.push(sri);
		this.StatusItemToAdd('');
		this.StatusItemMilestoneToAdd(0);
		this.StatusItemDateToAdd(new Date());
	};

	this.removeStatusItem = function (itemToRemove) {
		console.log("about to remove status item " + itemToRemove);
		if (ko.utils.unwrapObservable(itemToRemove.Id) != 0)
			self.ItemsToRemove.push(itemToRemove);
		// self.Items.remove(itemToRemove);
	};

	this.resurrectStatusItem = function (itemToResurrect) {
	    console.log("about to resurrect status item from StatusReport " + itemToResurrect);
	    self.ItemsToRemove.remove(itemToResurrect);
	};

	this.getProjectByName = function (name) {
		var proj = ko.utils.arrayFilter(this.Projects(), function (item) {
			return (item.Name == name);
		});
		return proj[0];
	};

	this.addItemViaQuickAdd = function () {
		var proj = self.getProjectByName(self.QuickAddProjectName());
		
		var statusItem = new statusReportItem()
		// .Report(self.Report)
		// in this case we don't know the project id, we may add later
			.ProjectId(proj.Id)
			.ProjectName(proj.Name)
			.ProjectTeamId(proj.TeamId)
			.ProjectTeamName(proj.TeamName)
			.ProjectLeadFullName(proj.LeadFullName)
            .HasInsertion(true)
		//.ProjectDepartmentName(self.ProjectDepartmentName())
		//.ProjectDepartmentManagerFullName(self.ProjectDepartmentManagerFullName())
		//.ProjectType(self.ProjectType())
		//.ProjectTeamId(self.ProjectTeamId())
		//.ProjectTeamName(self.ProjectTeamName())
		//.ProjectLeadFullName(self.ProjectLeadFullName())
		//.ProjectTeamLeadFullName(self.ProjectTeamLeadFullName())
		
			.TopicCaption(self.QuickAddCaption())
			.Caption(self.QuickAddCaption())
			.MilestoneDate(self.QuickAddMilestoneDate())
            .MilestoneType(self.QuickAddMilestoneType())
			.StatusReportId(self.Id())
			//.ProjectTeamId(self.TeamId())
			;

		statusItem.ListenForChanges();
		// we'll need to find the original project and team for this or else it goes to ether in UI, won't save etc
		self.loadStatusItem(statusItem);
		//self.addStatusItem(statusItem);
		self.QuickAddCaption(null);
		self.QuickAddMilestoneDate(new Date());
		self.QuickAddProjectName(null);
		$('#QuickAddCaptionText').focus();
	};

	this.HasNewQuickAddItem = ko.computed(function () {
		return (self.QuickAddCaption() != '' && self.QuickAddCaption() != null && self.QuickAddProjectName() != '' && self.QuickAddProjectName() != null);
	} .bind(this));
};

/**
Returns the names of all the obj's
variables and functions in a sorted
array
*/

//do some basic mapping (without mapping plugin)
//var mappedData = ko.utils.arrayMap(dataFromServer, function (item) {
//    return new Item(item.name, item.category, item.price);
//});

function getMembers(original) {
	var sri = new Array();
	$.each(original, function (index, item) {
		sri[index] = ko.utils.unwrapObservable(item);
	});
	return sri;
//    // Shallow copy
//	var clone = jQuery.extend({}, original);
//	// var b = obj.slice(0);//  jQuery.extend({}, obj);
//	return clone;
}

function statusReportItem() {
	var self = this;
	//self.Report = ko.observable(null);
	self.OriginalVersion = {};
	self.Id = ko.observable(0);
	self.TopicCaption = ko.observable('');
	self.TopicExternalId = ko.observable(null);
	self.TopicId = ko.observable(0);
	self.MilestoneType = ko.observable(1);
	self.MilestoneDate = ko.observable(new Date());
	self.MilestoneConfidenceLevel = ko.observable(0);
	self.Caption = ko.observable('');
	self.ProjectId = ko.observable(1);
	self.ProjectName = ko.observable('');
	self.ProjectDepartmentName = ko.observable('');
	self.ProjectDepartmentManagerFullName = ko.observable('');
	self.ProjectType = ko.observable(0);
	self.ProjectTeamId = ko.observable(0);
	self.ProjectTeamName = ko.observable('');
	self.ProjectLeadFullName = ko.observable('');
	self.ProjectTeamLeadFullName = ko.observable('');
	self.StatusReportId = ko.observable(0);
	self.TagsString = ko.observable('');

	this.LoadFromObject = function (item) {
		self.Id(item.Id)
		.TopicCaption(item.TopicCaption)
		.TopicExternalId(item.TopicExternalId)
		.TopicId(item.TopicId)
		.MilestoneType(item.MilestoneType)
		.MilestoneDate(item.MilestoneDate)
		.MilestoneConfidenceLevel(item.MilestoneConfidenceLevel)
		.Caption(item.Caption)
		.ProjectId(item.ProjectId)
		.ProjectName(item.ProjectName)
		.ProjectDepartmentName(item.ProjectDepartmentName)
		.ProjectDepartmentManagerFullName(item.ProjectDepartmentManagerFullName)
		.ProjectType(item.ProjectType)
		.ProjectTeamId(item.ProjectTeamId)
		.ProjectTeamName(item.ProjectTeamName)
		.ProjectLeadFullName(item.ProjectLeadFullName)
		.ProjectTeamLeadFullName(item.ProjectTeamLeadFullName)
        .TagsString(item.TagsString);

		self.OriginalVersion = getMembers(item);
		self.ListenForChanges();
		return self;
	};
	
	self.Subscribers = new Array();

	this.ClearSubscribers = function () {
		$.each(self.Subscribers, function (x, item) {
			item.dispose();
		});
		// clear change logs
		self.ChangeLog.removeAll();
	};

	self.MilestoneDateFormatted = ko.computed(function () {
		return parseJsonDateString(self.MilestoneDate());
	} .bind(this));

	self.ChangeLog = ko.observableArray();
	this.ListenForChanges = function () {
		self.ClearSubscribers();
		$.each(self, function (x, item) {
			if (!self.isInternal(x, item)) {
				var sub = item.subscribe(function (newValue) {
					var currentlyChangeExists = ($.inArray(x, self.ChangeLog()) >= 0);
					if (newValue != self.OriginalVersion[x] && !currentlyChangeExists) {
						console.log("The new value for " + x + " is " + newValue + " from " + self.OriginalVersion[x]);
						self.ChangeLog.push(x);
					} else if (currentlyChangeExists) {
						self.ChangeLog.pop(x);
					}
				});
				self.Subscribers.push(sub);
			}
		});
	};

	this.isInternal = function (x, item) {
	    return !(x != "ChangeLog" && x != "HasChanges" && x != "HasDeletion" && x != "HasInsertion" && x != "Editable" && ko.isObservable(item));
	};

	this.reset = function () {
	    // need to undo the value damage though
	    $.each(self, function (x, item) {
	        if (!self.isInternal(x, item)) {
	            var currentlyChangeExists = ($.inArray(x, self.ChangeLog()) >= 0);
	            if (currentlyChangeExists)
	                item(self.OriginalVersion[x]);
	            // self.Subscribers.push(sub);
	        }

	    });
	    self.ChangeLog.removeAll();
	    self.HasDeletion(false);
	    self.HasInsertion(false);
	};

    this.HasInsertion = ko.observable(false);

	this.HasChanges = ko.computed(function () {
		return self.ChangeLog().length > 0;
	} .bind(this));

    this.HasDeletion = ko.observable(false);

    this.Editable = ko.computed(function () {
        return (self.HasDeletion() == false);
    } .bind(this));

	this.MilestoneDateString = ko.computed(function () {
		return $.datepicker.formatDate('mm/dd/yy', self.MilestoneDate());
	} .bind(this));
};

function projectStatus() {
	var self = this;
	self.Report = ko.observable(null);
	self.ProjectId = ko.observable(1);
	self.ProjectName = ko.observable('');
	self.ProjectDepartmentName = ko.observable('');
	self.ProjectDepartmentManagerFullName = ko.observable('');
	self.ProjectType = ko.observable(0);
	self.ProjectTeamId = ko.observable(0);
	self.ProjectTeamName = ko.observable('');
	self.ProjectLeadFullName = ko.observable('');
	self.ProjectTeamLeadFullName = ko.observable('');
	self.Items = ko.observableArray([]);
	self.NewStatusItemText = ko.observable();
	self.NewStatusItemMilestoneDate = ko.observable(new Date());
    self.NewStatusItemMilestoneTypes = ko.observable(2);
	self.ItemsToRemove = ko.observableArray([]);

	this.HasNewItem = ko.computed(function () {
		return (self.NewStatusItemText() != '' && self.NewStatusItemText() != null);
	} .bind(this));

	self._destroyStatusItem = function (itemToRemove) {
	    console.log("destroying item: " + itemToRemove.TopicCaption());
	    self.Items.remove(itemToRemove);
	};

	self.removeStatusItem = function (itemToRemove) {

	    console.log("about to remove item via projectStatus " + itemToRemove.TopicCaption());
	    if (itemToRemove.HasInsertion()) {
	        itemToRemove.reset();
	        self.Items.remove(itemToRemove);
            self.Report().removeStatusItem(itemToRemove);
	        
	    } else {
	        itemToRemove.reset();
	        itemToRemove.HasDeletion(true);
	        self.ItemsToRemove.push(itemToRemove);
	        self.Report().removeStatusItem(itemToRemove);
	    }
	};

	self.resurrectStatusItem = function (itemToResurrect) {
	    console.log("about to resurrect status item " + itemToResurrect);
	    self.ItemsToRemove.remove(itemToResurrect);
	    itemToResurrect.HasDeletion(false);
	    self.Report().resurrectStatusItem(itemToResurrect);
	    // self.Items.remove(itemToRemove);
	};

	self.addItem = function (statusItem) {
		self.Items.push(statusItem);
		self.Report().addStatusItem(statusItem);
		$(".datefield").datepicker({ dateFormat: 'yy-mm-dd', changeMonth: true, changeYear: true });
};
	
	self.addItemFromTemplate = function () {
		var statusItem = new statusReportItem()
		//.Report(self.Report)
			.ProjectId(self.ProjectId())
			.ProjectName(self.ProjectName())
			.ProjectDepartmentName(self.ProjectDepartmentName())
			.ProjectDepartmentManagerFullName(self.ProjectDepartmentManagerFullName())
			.ProjectType(self.ProjectType())
			.ProjectTeamId(self.ProjectTeamId())
			.ProjectTeamName(self.ProjectTeamName())
			.ProjectLeadFullName(self.ProjectLeadFullName())
			.ProjectTeamLeadFullName(self.ProjectTeamLeadFullName())
			;
		
		// needs to be treated as new anyway
		self.addItem(statusItem);

	};
	self.addItemFromStubProperties = function () {
		var statusItem = new statusReportItem()
		// .Report(self.Report)
			.ProjectId(self.ProjectId())
			.ProjectName(self.ProjectName())
			.ProjectDepartmentName(self.ProjectDepartmentName())
			.ProjectDepartmentManagerFullName(self.ProjectDepartmentManagerFullName())
			.ProjectType(self.ProjectType())
			.ProjectTeamId(self.ProjectTeamId())
			.ProjectTeamName(self.ProjectTeamName())
			.ProjectLeadFullName(self.ProjectLeadFullName())
			.ProjectTeamLeadFullName(self.ProjectTeamLeadFullName())
			.TopicCaption(self.NewStatusItemText())
			.Caption(self.NewStatusItemText())
			.MilestoneDate(self.NewStatusItemMilestoneDate())
            .MilestoneType(self.NewStatusItemMilestoneTypes())
			.StatusReportId(self.Report().Id());
		statusItem.ListenForChanges();
		self.addItem(statusItem);
		self.NewStatusItemText('');
		self.NewStatusItemMilestoneDate(new Date());
	};

    this.PendingChangesCount = ko.computed(function () {
        var arrCount = ko.utils.arrayFilter(self.Items(), function (item) {
            return item.HasChanges();
        });

        // debugging
        ko.utils.arrayForEach(self.Items(), function (item) {
            if (item.HasChanges())
                console.log(ko.toJSON(item));
        });

        return arrCount.length;
    } .bind(this));

    this.PendingDeletionsCount = ko.computed(function () {
        var arrCount = ko.utils.arrayFilter(self.Items(), function (item) {
            return item.HasDeletion();
        });

        // debugging
        ko.utils.arrayForEach(self.Items(), function (item) {
            if (item.HasDeletion())
                console.log(ko.toJSON(item));
        });

        return arrCount.length;
    } .bind(this));
};

function teamStatus() {
    var self = this;
	this.Report = ko.observable(null);
	this.TeamId = ko.observable(0);
	this.Name = ko.observable('');
	this.ProjectItems = ko.observableArray([]);

	this.HasChanges = ko.computed(function () {
	    if (!this.PendingChangesCount) return false;
	    return (this.PendingChangesCount() > 0 || this.PendingDeletionsCount() > 0);
	} .bind(this));

	this.PendingChangesCount = ko.computed(function () {
	    // debugging
	    var count = 0;
	    ko.utils.arrayForEach(self.ProjectItems(), function (item) {
	        count += item.PendingChangesCount();
	    });

	    return count;
	} .bind(this));

	this.PendingDeletionsCount = ko.computed(function () {
	    var count = 0;
	    ko.utils.arrayForEach(self.ProjectItems(), function (item) {
	        count += item.PendingDeletionsCount();
	    });

	    return count;
	} .bind(this));

	this.addProject = function (project) {
		var projects = ($.grep(this.ProjectItems(), function (i) {
			return (i.ProjectId() == project.ProjectId());
		}));
		if (projects.length == 0)
			this.ProjectItems.push(project);
	};
}