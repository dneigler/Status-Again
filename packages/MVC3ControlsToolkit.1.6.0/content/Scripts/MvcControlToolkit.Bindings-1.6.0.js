/* ****************************************************************************
*  MvcControlToolkit.Bindings-1.5.0.js
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/
///////////////////////ClientViewModel Methods definition//////////
function MvcControlsToolkit_ClientViewModel_Init(viewModel, jsonHiddenId, validationType) {
    viewModel.save = function () {
        document.getElementById(jsonHiddenId).value = ko.mapping.toJSON(this);
    };
    viewModel.validateAndSave = function () {
        if (MvcControlsToolkit_FormIsValid(jsonHiddenId, validationType)) {
            document.getElementById(jsonHiddenId).value = ko.mapping.toJSON(this);
            return true;
        }
        return false;
    }
    viewModel.saveAndSubmit = function () {
        if (this.validateAndSave()) {
            $('#' + jsonHiddenId).parents('form').submit();
        }
    };
    viewModel.saveAndSubmitAlone = function (formId) {
        if (MvcControlsToolkit_FormIsValid(formId, validationType)) {
            this.save();
            $('#' + jsonHiddenId).parents('form').submit();
        }
    }
    $(document).ready(function () {
        $('#' + jsonHiddenId).parents('form').submit(function () {
            viewModel.save();
            return true;
        });
    });
}
///////////////////////Template Names//////////////////////////////

function MvcControlsToolkit_NewTemplateName(item) {
    if (eval("typeof _MvcControlsToolkit_" + item.ModelId + " === 'undefined'")) {
        jQuery.globalEval("var _MvcControlsToolkit_" + item.ModelId + " = 0;");
    }
    else {
        eval("_MvcControlsToolkit_" + item.ModelId + "++ ;");
    }
    return "";
}
function MvcControlsToolkit_TemplateName(item) {
    return item.ModelPrefix + "[" + eval("_MvcControlsToolkit_" + item.ModelId) + "]" + item.ItemPrefix;
}
function MvcControlsToolkit_TemplateId(item) {
    return item.ModelId + "_" + eval("_MvcControlsToolkit_" + item.ModelId) + "_";
}
///////////////////////////Templates/////////////////////////////
function MvcControlsToolkit_InitializeCreatedNodes(elements, unobtrusiveValidation, noInput, noJavaScript, applyValidation, delayedBatch) {
    if (elements == null || (noJavaScript && noInput)) return;

    toApply = function () {
        for (var i = 0; i < elements.length; i++) {
            var jel = $(elements[i]);
            if (jel.data('isCached') || false) return;
            if (unobtrusiveValidation && applyValidation) {
                jQuery.validator.unobtrusive.parseExt(elements[i]);
            }
            if (!noJavaScript) GlobalEvalScriptAndDestroy(elements[i]);
            if (!noInput) {
                var sons = jel.children();
                sons.trigger('blur');
                sons.trigger('change');
            }

        }
    };
    setTimeout(toApply, 0);
}

function MvcControlsToolkit_ServerErrors(elements) {
    var ToApply = function () {
        for (var i = 0; i < elements.length; i++) {
            var currElement = elements[i];
            var currDom = $('#' + currElement.id);
            if (currDom.length == 0) continue;
            var currForm = currDom.parents('form').first();
            if (currForm.length == 0) continue;

            if (!currDom.hasClass('input-validation-error'))
                currDom.addClass('input-validation-error');
            var attr = currDom.attr('companionpostfix');
            if (typeof attr !== 'undefined' && attr !== false) {
                var companion = $('#' + currElement.id + attr);
                if (companion.length > 0 && !companion.hasClass('input-validation-error'))
                    companion.addClass('input-validation-error');
            }
            var currDisplay = $(currForm).find("[data-valmsg-for='" + currElement.name + "']");
            if (currDisplay.length > 0) {
                currDisplay.removeClass("field-validation-valid").addClass("field-validation-error");
                replace = $.parseJSON(currDisplay.attr("data-valmsg-replace")) !== false;
                if (replace) {
                    currDisplay.empty();
                    $(currElement.errors[0]).appendTo(currDisplay);
                }
            }
        }
    };
    setTimeout(ToApply, 0);
}

(function () {
    var mappingProperty = "__ko_mapping__";
    ko.mapping.toJS = function (rootObject, options) {
        if (arguments.length == 0) throw new Error("When calling ko.mapping.toJS, pass the object you want to convert.");

        options = options || {};
        options.ignore = options.ignore || [];
        if (!(options.ignore instanceof Array)) {
            options.ignore = [options.ignore];
        }

        options.ignore.push(mappingProperty);

        // We just unwrap everything at every level in the object graph
        return ko.mapping.visitModel(rootObject, function (x) {
            var res = ko.utils.unwrapObservable(x);
            if (Object.prototype.toString.call(res) === '[object Date]')
                res = '\\/Date(' + res.getTime() + ')\\/';
            return res;
        }, options);
    };
})();
ko.utils.arrayBestIndexOf = function (array, item, isFloat) {
    if (!isFloat) return ko.utils.arrayIndexOf(array, item);
    var j = array.length;
    if (j == 0) return -1;
    if (item === undefined) return 0;
    var besterror = Math.abs(array[0] - item);
    var bestIndex = 0;
    if (array[0] === undefined) {
        if (array.length <= 1) return -1;
        besterror = Math.abs(array[1] - item);
        bestIndex = 1;
    }
    var currError;
    for (var i = 1; i < j; i++) {
        currError = Math.abs(array[i] - item);
        if (currError < besterror) {
            bestIndex = i;
            besterror = currError;
        }
    }
    return bestIndex;
};
ko.bindingHandlers.valueExt = {
    'init': function (element, valueAccessor, allBindingsAccessor) {
        var eventName = allBindingsAccessor()["valueUpdate"] || "change";
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var elementType = $(element).attr("element-type") || "";
        if (eventName == "change" && element.tagName != "SELECT" && element.tagName != "CHECKBOX")
            eventName = "blur";
        if (elementType != "") eventName = elementType + "_Changed";

        // The syntax "after<eventname>" means "run the handler asynchronously after the event"
        // This is useful, for example, to catch "keydown" events after the browser has updated the control
        // (otherwise, ko.selectExtensions.readValue(this) will receive the control's value *before* the key event)
        var handleEventAsynchronously = false;
        if (elementType == "" && eventName.length > 7 && ko.utils.stringStartsWith(eventName, "after")) {
            handleEventAsynchronously = true;
            eventName = eventName.substring("after".length);
        }
        var runEventHandler = handleEventAsynchronously ? function (handler) { setTimeout(handler, 0) }
                                                        : function (handler) { handler() };

        ko.utils.registerEventHandler(element, eventName, function () {
            runEventHandler(function () {
                var modelValue = valueAccessor();
                var elementValue = null;
                if (elementType != "") {
                    elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
                }
                else {
                    elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
                }
                if (isNaN(elementValue) && elementType == "") elementValue = ko.selectExtensions.readValue(element);
                if (ko.isWriteableObservable(modelValue))
                    modelValue(elementValue);
                else {
                    var allBindings = allBindingsAccessor();
                    if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['value'])
                        allBindings['_ko_property_writers']['value'](elementValue);
                }
            });
        });
    },
    'update': function (element, valueAccessor, allBindingsAccessor) {
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var formatString = allBindingsAccessor()["formatString"] || '';
        var elementType = $(element).attr("element-type") || "";
        if (elementType != "") eventName = elementType + "_changed";

        var newValue = ko.utils.unwrapObservable(valueAccessor());

        var elementValue = null;
        if (elementType != "") {
            elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
        }
        else {
            elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
        }
        if (isNaN(elementValue) && elementType == "") elementValue = ko.selectExtensions.readValue(element);
        var valueHasChanged = (newValue != elementValue);
        if (elementValue instanceof Array) {
            if (newValue instanceof Array) {
                if (newValue.length != elementValue.length) valueHasChanged = true;
                else {
                    valueHasChanged = false;
                    for (var i = 0, j = newValue.length; i < j; i++) {
                        if (newValue[i] != elementValue[i]) {
                            valueHasChanged = true;
                            break;
                        }
                    }
                }
            }
            else valueHasChanged = true;
        }
        // JavaScript's 0 == "" behavious is unfortunate here as it prevents writing 0 to an empty text box (loose equality suggests the values are the same). 
        // We don't want to do a strict equality comparison as that is more confusing for developers in certain cases, so we specifically special case 0 != "" here.
        else if ((newValue === 0) && (elementValue !== 0) && (elementValue !== "0"))
            valueHasChanged = true;

        if (valueHasChanged) {
            var convertedValue = null;
            var applyValueAction = null;
            if (elementType != "") {
                applyValueAction = function () {
                    eval("MvcControlsToolkit_" + elementType + "_Set(element, newValue, formatString, valueType);");

                };
            }
            else {
                convertedValue = MvcControlsToolkit_ToString(newValue, formatString, valueType);
                applyValueAction = function () {
                    if (element.tagName == 'SELECT' && valueType == MvcControlsToolkit_DataType_Float) {
                        if (element.options.length > 0) {
                            var besterror = Math.abs(MvcControlsToolkit_Parse(ko.selectExtensions.readValue(element.options[0]), valueType) - newValue);
                            var bestIndex = 0;
                            for (var i = element.options.length - 1; i >= 0; i--) {
                                var currError = Math.abs(MvcControlsToolkit_Parse(ko.selectExtensions.readValue(element.options[i]), valueType) - newValue);
                                if (currError < besterror) {
                                    besterror = currError;
                                    bestIndex = i;
                                }
                            }
                            element.selectedIndex = bestIndex;
                        }
                    }
                    else
                        ko.selectExtensions.writeValue(element, convertedValue);
                    ko.utils.triggerEvent(element, "blur");
                };
            }
            applyValueAction();

            // Workaround for IE6 bug: It won't reliably apply values to SELECT nodes during the same execution thread
            // right after you've changed the set of OPTION nodes on it. So for that node type, we'll schedule a second thread
            // to apply the value as well.
            var alsoApplyAsynchronously = element.tagName == "SELECT";
            if (alsoApplyAsynchronously)
                setTimeout(applyValueAction, 0);
        }

        // For SELECT nodes, you're not allowed to have a model value that disagrees with the UI selection, so if there is a
        // difference, treat it as a change that should be written back to the model

        if (element.tagName == "SELECT") {
            if (elementType != "") {
                elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
            }
            else {
                elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
            }
            if (elementValue !== newValue)
                ko.utils.triggerEvent(element, "change");
        }

    }
};
ko.bindingHandlers.selectedOptionsExt = {
    getSelectedValuesFromSelectNode: function (selectNode, valueType, all) {
        var result = [];
        var nodes = selectNode.childNodes;
        for (var i = 0, j = nodes.length; i < j; i++) {
            var node = nodes[i];
            if (node.tagName == "OPTGROUP") {
                var opts = node.childNodes;
                for (var n = 0, l = opts.length; n < l; n++) {
                    var opt = opts[n];
                    if ((opt.tagName == "OPTION") && (opt.selected || all != null))
                        result.push(MvcControlsToolkit_Parse(ko.selectExtensions.readValue(opt), valueType));
                }
            }
            else if ((node.tagName == "OPTION") && (node.selected || all != null))
                result.push(MvcControlsToolkit_Parse(ko.selectExtensions.readValue(node), valueType));
        }
        return result;
    },
    'init': function (element, valueAccessor, allBindingsAccessor) {
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        ko.utils.registerEventHandler(element, "change", function () {
            var value = valueAccessor();
            var newValue = ko.bindingHandlers.selectedOptionsExt.getSelectedValuesFromSelectNode(this, valueType);
            if (ko.isWriteableObservable(value))
                value(newValue);
            else {
                var allBindings = allBindingsAccessor();
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['value'])
                    allBindings['_ko_property_writers']['value'](newValue);
            }
        });
    },
    'update': function (element, valueAccessor, allBindingsAccessor) {
        if (element.tagName != "SELECT")
            throw new Error("values binding applies only to SELECT elements");
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var newValue = ko.utils.unwrapObservable(valueAccessor());
        if (newValue && typeof newValue.length == "number") {
            var allElements = ko.bindingHandlers.selectedOptionsExt.getSelectedValuesFromSelectNode(element, valueType, true);
            var chosenIdexes = [];
            for (var i = 0, j = newValue.length; i < j; i++) chosenIdexes.push(ko.utils.arrayBestIndexOf(allElements, newValue[i], (valueType == MvcControlsToolkit_DataType_Float)));
            var nodes = element.childNodes;
            var opCount = 0;
            for (var i = 0, j = nodes.length; i < j; i++) {
                var node = nodes[i];
                if (node.tagName == "OPTGROUP") {
                    var opts = node.childNodes;
                    for (var n = 0, l = opts.length; n < l; n++) {
                        var opt = opts[n];
                        if (opt.tagName == "OPTION") {

                            opt.selected = ko.utils.arrayBestIndexOf(chosenIdexes, opCount, false) >= 0;
                            opCount++;
                        }
                    }
                }
                else if (node.tagName == "OPTION") {

                    node.selected = ko.utils.arrayBestIndexOf(chosenIdexes, opCount, false) >= 0;
                    opCount++;
                }
            }
        }
    }
};
ko.bindingHandlers.checkedExt = {
    'init': function (element, valueAccessor, allBindingsAccessor) {
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var updateHandler = function () {
            var valueToWrite;
            if (element.type == "checkbox") {
                valueToWrite = element.checked;
            } else if ((element.type == "radio") && (element.checked)) {
                valueToWrite = MvcControlsToolkit_Parse(element.value, valueType);
            } else {
                return; // "checked" binding only responds to checkboxes and selected radio buttons
            }

            var modelValue = valueAccessor();
            if ((element.type == "checkbox") && (ko.utils.unwrapObservable(modelValue) instanceof Array)) {
                // For checkboxes bound to an array, we add/remove the checkbox value to that array
                // This works for both observable and non-observable arrays
                var elementValue = MvcControlsToolkit_Parse(element.value, valueType);
                var existingEntryIndex = ko.utils.arrayIndexOf(ko.utils.unwrapObservable(modelValue), MvcControlsToolkit_Parse(elementValue, valueType));
                if (element.checked && (existingEntryIndex < 0))
                    modelValue.push(elementValue);
                else if ((!element.checked) && (existingEntryIndex >= 0))
                    modelValue.splice(existingEntryIndex, 1);
            } else if (ko.isWriteableObservable(modelValue)) {
                if (modelValue() !== valueToWrite) { // Suppress repeated events when there's nothing new to notify (some browsers raise them)
                    modelValue(valueToWrite);
                }
            } else {
                var allBindings = allBindingsAccessor();
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['checked']) {
                    allBindings['_ko_property_writers']['checked'](valueToWrite);
                }
            }
            return true;
        };
        ko.utils.registerEventHandler(element, "click", updateHandler);

        // IE 6 won't allow radio buttons to be selected unless they have a name
        if ((element.type == "radio") && !element.name)
            ko.bindingHandlers['uniqueName']['init'](element, function () { return true });
    },
    'update': function (element, valueAccessor, allBindingsAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        if (element.type == "checkbox") {
            if (value instanceof Array) {
                // When bound to an array, the checkbox being checked represents its value being present in that array
                element.checked = ko.utils.arrayIndexOf(value, MvcControlsToolkit_Parse(element.value, valueType)) >= 0;
            } else {
                // When bound to anything other value (not an array), the checkbox being checked represents the value being trueish
                element.checked = value;
            }

            // Workaround for IE 6 bug - it fails to apply checked state to dynamically-created checkboxes if you merely say "element.checked = true"
            if (value && ko.utils.isIe6)
                element.mergeAttributes(document.createElement("<input type='checkbox' checked='checked' />"), false);
        } else if (element.type == "radio") {
            var nodeValue = MvcControlsToolkit_Parse(element.value, valueType);
            element.checked = (nodeValue == value);

            // Workaround for IE 6/7 bug - it fails to apply checked state to dynamically-created radio buttons if you merely say "element.checked = true"
            if ((nodeValue == value) && (ko.utils.isIe6 || ko.utils.isIe7))
                element.mergeAttributes(document.createElement("<input type='radio' checked='checked' />"), false);
        }
    }
};
function MvcControlsToolkit_GetArrayString(value, arrayName, isNullable) {
    var allValues = eval(arrayName);
    if (value === null) return allValues[0];
    var index = 0;
    if (value === false) index = 0;
    else if (value === true) index = 1;
    else index = value;
    if (isNullable) index++;
    return allValues[index];
}
ko.bindingHandlers['template']["getCachedNodes"] = function (value) { return null; };
ko.jqueryTmplTemplateEngineExt = function () {
    // Detect which version of jquery-tmpl you're using. Unfortunately jquery-tmpl 
    // doesn't expose a version number, so we have to infer it.
    this.jQueryTmplVersion = (function () {
        if ((typeof (jQuery) == "undefined") || !jQuery['tmpl'])
            return 0;
        // Since it exposes no official version number, we use our own numbering system. To be updated as jquery-tmpl evolves.
        if (jQuery['tmpl']['tag']) {
            if (jQuery['tmpl']['tag']['tmpl'] && jQuery['tmpl']['tag']['tmpl']['open']) {
                if (jQuery['tmpl']['tag']['tmpl']['open'].toString().indexOf('__') >= 0) {
                    return 3; // Since 1.0.0pre, custom tags should append markup to an array called "__"
                }
            }
            return 2; // Prior to 1.0.0pre, custom tags should append markup to an array called "_"
        }
        return 1; // Very old version doesn't have an extensible tag system
    })();

    function getTemplateNode(template) {
        var templateNode = document.getElementById(template);
        if (templateNode == null)
            throw new Error("Cannot find template with ID=" + template);
        return templateNode;
    }

    // These two only needed for jquery-tmpl v1
    var aposMarker = "__ko_apos__";
    var aposRegex = new RegExp(aposMarker, "g");

    this['renderTemplate'] = function (templateId, data, options) {
        options = options || {};
        var cachedNodes = ko.bindingHandlers['template']['getCachedNodes'](data);
        if (cachedNodes != null) {
            $(cachedNodes).data('isCached', true);
            return cachedNodes;
        }
        if (this.jQueryTmplVersion == 0)
            throw new Error("jquery.tmpl not detected.\nTo use KO's default template engine, reference jQuery and jquery.tmpl. See Knockout installation documentation for more details.");

        if (this.jQueryTmplVersion == 1) {
            // jquery.tmpl v1 doesn't like it if the template returns just text content or nothing - it only likes you to return DOM nodes.
            // To make things more flexible, we can wrap the whole template in a <script> node so that jquery.tmpl just processes it as
            // text and doesn't try to parse the output. Then, since jquery.tmpl has jQuery as a dependency anyway, we can use jQuery to
            // parse that text into a document fragment using jQuery.clean().        
            var templateTextInWrapper = "<script type=\"text/html\">" + getTemplateNode(templateId).text + "</script>";
            var renderedMarkupInWrapper = jQuery['tmpl'](templateTextInWrapper, data);
            var renderedMarkup = renderedMarkupInWrapper[0].text.replace(aposRegex, "'"); ;
            var finalRes = jQuery['clean']([renderedMarkup], document);
            //renderedMarkup = finalRes.text();
            //finalRes = jQuery['clean']([renderedMarkup], document);
            return finalRes;
        }

        // It's easier with jquery.tmpl v2 and later - it handles any DOM structure
        if (!(templateId in jQuery['template'])) {
            // Precache a precompiled version of this template (don't want to reparse on every render)
            var templateText = getTemplateNode(templateId).text;
            jQuery['template'](templateId, templateText);
        }
        data = [data]; // Prewrap the data in an array to stop jquery.tmpl from trying to unwrap any arrays

        var resultNodes = jQuery['tmpl'](templateId, data, options['templateOptions']);
        resultNodes['appendTo'](document.createElement("div")); // Using "appendTo" forces jQuery/jQuery.tmpl to perform necessary cleanup work
        //var renderedMarkup = resultNodes.text();
        //resultNodes = jQuery['clean']([renderedMarkup], document);
        jQuery['fragments'] = {}; // Clear jQuery's fragment cache to avoid a memory leak after a large number of template renders

        return resultNodes;
    },

    this['isTemplateRewritten'] = function (templateId) {
        // It must already be rewritten if we've already got a cached version of it
        // (this optimisation helps on IE < 9, because it greatly reduces the number of getElementById calls)
        if (templateId in jQuery['template'])
            return true;

        return getTemplateNode(templateId).isRewritten === true;
    },

    this['rewriteTemplate'] = function (template, rewriterCallback) {
        var templateNode = getTemplateNode(template);
        var templateNodeText = templateNode.text.replace(/agkcvriopjvss/g, "script");
        var rewritten = rewriterCallback(templateNodeText);

        if (this.jQueryTmplVersion == 1) {
            // jquery.tmpl v1 falls over if you use single-quotes, so replace these with a temporary marker for template rendering, 
            // and then replace back after the template was rendered. This is slightly complicated by the fact that we must not interfere
            // with any code blocks - only replace apos characters outside code blocks.
            rewritten = ko.utils.stringTrim(rewritten);
            rewritten = rewritten.replace(/([\s\S]*?)(\${[\s\S]*?}|{{[\=a-z][\s\S]*?}}|$)/g, function (match) {
                // Called for each non-code-block followed by a code block (or end of template)
                var nonCodeSnippet = arguments[1];
                var codeSnippet = arguments[2];
                return nonCodeSnippet.replace(/\'/g, aposMarker) + codeSnippet;
            });
        }

        templateNode.text = '${MvcControlsToolkit_NewTemplateName($item) } ' + rewritten;
        templateNode.isRewritten = true;
    },

    this['createJavaScriptEvaluatorBlock'] = function (script) {
        if (this.jQueryTmplVersion == 1)
            return "{{= " + script + "}}";

        // From v2, jquery-tmpl does some parameter parsing that fails on nontrivial expressions.
        // Prevent it from messing with the code by wrapping it in a further function.
        return "{{ko_code ((function() { return " + script + " })()) }}";
    },

    this.addTemplate = function (templateName, templateMarkup) {
        document.write("<script type='text/html' id='" + templateName + "'>" + templateMarkup + "</script>");
    }
    ko.exportProperty(this, 'addTemplate', this.addTemplate);

    if (this.jQueryTmplVersion > 1) {
        jQuery['tmpl']['tag']['ko_code'] = {
            open: (this.jQueryTmplVersion < 3 ? "_" : "__") + ".push($1 || '');"
        };
    }
};

ko.jqueryTmplTemplateEngineExt.prototype = new ko.templateEngine();

// Use this one by default
ko.setTemplateEngine(new ko.jqueryTmplTemplateEngineExt());
ko.bindingHandlers['template']['originalUpdate'] = ko.bindingHandlers['template']['update'];
ko.bindingHandlers['template']['update'] = function (element, valueAccessor, allBindingsAccessor, viewModel) {
    var bindingValue = ko.utils.unwrapObservable(valueAccessor());
    ko.bindingHandlers['template']['originalUpdate'](element, valueAccessor, allBindingsAccessor, viewModel);
    var oldDO = $(element).data("__mvcct_template_afterRender__") || null;
    if (oldDO != null && (typeof (oldDO.dispose) == 'function')) oldDO.dispose();
    $(element).data("__mvcct_template_afterRender__", null);
    if (typeof bindingValue['afterAllRender'] == 'function') {
        var dependentObservable = new ko.dependentObservable(
            function () {
                var unwrappedArray = ko.utils.unwrapObservable(bindingValue['foreach']);
                bindingValue['afterAllRender'](element, viewModel);
            },
            null,
            { 'disposeWhenNodeIsRemoved': element });
        $(element).data("__mvcct_template_afterRender__", dependentObservable)
    }
}
ko.exportSymbol('ko.jqueryTmplTemplateEngineExt', ko.jqueryTmplTemplateEngineExt);

ko.bindingHandlers['options']['originalUpdate'] = ko.bindingHandlers['options']['update'];
ko.bindingHandlers['options']['update'] = function (element, valueAccessor, allBindingsAccessor, viewModel) {
    var value = ko.utils.unwrapObservable(valueAccessor());
    if (typeof value.length != "number") value = [value];
    var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
    var formatString = allBindingsAccessor()["formatString"] || '';
    var textType = allBindingsAccessor()["textType"] || MvcControlsToolkit_DataType_String;
    var textFormatString = allBindingsAccessor()["textFormatString"] || '';
    var textPrefix = allBindingsAccessor()["textPrefix"] || '';
    var textPostfix = allBindingsAccessor()["textPostfix"] || '';
    var textNullString = allBindingsAccessor()["textNullString"] || '';
    var convertedValues = [];
    var allBindings = allBindingsAccessor();
    for (var i = 0, j = value.length; i < j; i++) {
        if (typeof allBindings['optionsValue'] == "string") {
            var optionValue = value[i][allBindings['optionsValue']];
            optionValue = ko.utils.unwrapObservable(optionValue);
            var optionsTextValue = allBindings['optionsText'];
            var item = {};
            item[allBindings['optionsValue']] = MvcControlsToolkit_ToString(ko.utils.unwrapObservable(value[i][allBindings['optionsValue']]), formatString, valueType)
            if (typeof optionsTextValue == "string") {
                var textValue = ko.utils.unwrapObservable(ko.utils.unwrapObservable(value[i])[optionsTextValue]);
                if (textValue === null)
                    item[optionsTextValue] = textNullString;
                else
                    item[optionsTextValue] = textPrefix + MvcControlsToolkit_ToString(textValue, textFormatString, textType) + textPostfix;
            }
            convertedValues.push(item);
        }
        else {

            convertedValues.push(MvcControlsToolkit_ToString(ko.utils.unwrapObservable(value[i]), formatString, valueType));
        }

    }
    ko.bindingHandlers['options']['originalUpdate'](element, function () { return convertedValues; }, allBindingsAccessor, viewModel);

}