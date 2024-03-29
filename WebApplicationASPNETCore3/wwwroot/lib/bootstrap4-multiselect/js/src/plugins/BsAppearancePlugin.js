import {closestByClassName, siblingsAsArray} from '../ToolsDom';
import {addStyling} from '../ToolsStyling'
import {ObservableLambda, composeSync} from '../ToolsJs';

export function BsAppearancePlugin(pluginData){
    let {configuration, common, staticContent} = pluginData;
    let {getValidity, getSize, useCssPatch, css} = configuration;
    let selectElement = staticContent.selectElement;
    
    if (staticContent.getLabelElement){
        let origGetLabelElement = staticContent.getLabelElement;
        staticContent.getLabelElement = () => {
            var e = origGetLabelElement();
            if (e)
                return e;
            else
                return getLabelElement(selectElement);
        }
    }
    
    if (staticContent.selectElement) {
        if(!getValidity)
            getValidity = composeGetValidity(selectElement);
        if(!getSize) 
            getSize = composeGetSize(selectElement);
    } else {
        if (!getValidity)
            getValidity = () => null
        if (!getSize)
            getSize = () => null;
    }

    common.getSize=getSize;
    common.getValidity=getValidity;
    return {
        afterConstructor(multiSelect){
            
            var updateSize;
            if (!useCssPatch){
                updateSize= () => updateSizeForAdapter(staticContent.picksElement, getSize)
            }
            else{
                const {picks_lg, picks_sm, picks_def} = css;
                updateSize = () => updateSizeJsForAdapter(staticContent.picksElement, picks_lg, picks_sm, picks_def, getSize);
            }
            multiSelect.UpdateSize = updateSize;
            
            if (useCssPatch){
                var defToggleFocusStyling = staticContent.toggleFocusStyling;
                staticContent.toggleFocusStyling = () => {
                    var validity =  validationObservable.getValue();
                    var isFocusIn = staticContent.getIsFocusIn();
                    defToggleFocusStyling(isFocusIn)
                    if (isFocusIn){
                        if (validity===false) { 
                            // but not toggle events (I know it will be done in future)
                            staticContent.setIsFocusIn(isFocusIn);
                            
                            addStyling(staticContent.picksElement, css.picks_focus_invalid)
                        } else if (validity===true) {
                            // but not toggle events (I know it will be done in future)
                            staticContent.setIsFocusIn(isFocusIn);
                            
                            addStyling(staticContent.picksElement, css.picks_focus_valid)  
                        }              
                    }
                }
            }
        
            var getWasValidated = () => {
                var wasValidatedElement = closestByClassName(staticContent.initialElement, 'was-validated');
                return wasValidatedElement?true:false;
            }
            var wasUpdatedObservable = ObservableLambda(()=>getWasValidated());
            var getManualValidationObservable = ObservableLambda(()=>getValidity());
            let validationApiObservable = staticContent.validationApiObservable;
            
            var validationObservable = ObservableLambda(
                () => wasUpdatedObservable.getValue()?validationApiObservable.getValue():getManualValidationObservable.getValue()
            )
          
            validationObservable.attach(
                (value)=>{
                    var  {validMessages, invalidMessages} = getMessagesElements(staticContent.containerElement);
                    updateValidity( 
                    staticContent.picksElement,
                    validMessages, invalidMessages,
                    value);
                    staticContent.toggleFocusStyling();
                }
            )
            wasUpdatedObservable.attach(
                ()=>validationObservable.call()
            )
            validationApiObservable.attach(
                ()=>validationObservable.call()
            )
            getManualValidationObservable.attach(
                ()=>validationObservable.call()
            )
               
            multiSelect.UpdateValidity = ()=> getManualValidationObservable.call();
            multiSelect.UpdateWasValidated = ()=>wasUpdatedObservable.call();
            
            multiSelect.updateAppearance = composeSync(
                multiSelect.updateAppearance.bind(multiSelect), 
                updateSize, 
                validationObservable.call, getManualValidationObservable.call);
            
            return /* dispose */() => {
                wasUpdatedObservable.detachAll();
                validationObservable.detachAll();
                getManualValidationObservable.detachAll();
            }
        }
    }
}

function getLabelElement(selectElement){
    let value = null;
    let formGroup = closestByClassName(selectElement,'form-group');
    if (formGroup) {
        value = formGroup.querySelector(`label[for="${selectElement.id}"]`);
    }
    return value;
}

function updateValidity(picksElement, validMessages, invalidMessages, validity){
    if (validity===false){
        picksElement.classList.add('is-invalid');
        picksElement.classList.remove('is-valid');
        invalidMessages.map(e=>e.style.display='block'); 
        validMessages.map(e=>e.style.display='none');      
    }
    else if (validity===true){
        picksElement.classList.remove('is-invalid');
        picksElement.classList.add('is-valid');
        invalidMessages.map(e=>e.style.display='none'); 
        validMessages.map(e=>e.style.display='block'); 
    }else {
        picksElement.classList.remove('is-invalid');
        picksElement.classList.remove('is-valid');
        invalidMessages.map(e=>e.style.display=''); 
        validMessages.map(e=>e.style.display=''); 
    }
}

function updateSize(picksElement, size){
    if (size=="lg"){
        picksElement.classList.add('form-control-lg');
        picksElement.classList.remove('form-control-sm');
    }
    else if (size=="sm"){
        picksElement.classList.remove('form-control-lg');
        picksElement.classList.add('form-control-sm');
    }
    else{
        picksElement.classList.remove('form-control-lg');
        picksElement.classList.remove('form-control-sm');
    }
}

function updateSizeJs(picksElement, picksLgStyling, picksSmStyling, picksDefStyling, size){
    updateSize(picksElement, size);
    if (size=="lg"){
        addStyling(picksElement, picksLgStyling);
    } else if (size=="sm"){
        addStyling(picksElement, picksSmStyling);
    } else {
        addStyling(picksElement, picksDefStyling);
    }
}

function updateSizeForAdapter(picksElement, getSize){
    updateSize(picksElement, getSize())
}

function updateSizeJsForAdapter(picksElement, picksLgStyling, picksSmStyling, picksDefStyling, getSize){
    updateSizeJs(picksElement, picksLgStyling, picksSmStyling, picksDefStyling, getSize())
}

function getMessagesElements(containerElement){
    var siblings = siblingsAsArray(containerElement);
    var invalidMessages =  siblings.filter(e=>e.classList.contains('invalid-feedback') || 
        e.classList.contains('invalid-tooltip'));
    var validMessages =  siblings.filter(e=>e.classList.contains('valid-feedback') || 
        e.classList.contains('valid-tooltip'));
    return {validMessages, invalidMessages}    
} 

function composeGetValidity(selectElement){
    var getValidity = () => 
        selectElement.classList.contains('is-invalid')?false:
        (selectElement.classList.contains('is-valid')?true:null);
    return getValidity;
}

function composeGetSize(selectElement){
    let inputGroupElement = closestByClassName(selectElement, 'input-group');
    let getSize = null;
    if (inputGroupElement){
        getSize = function(){
            var value = null;
            if (inputGroupElement.classList.contains('input-group-lg'))
                value = 'lg';
            else if (inputGroupElement.classList.contains('input-group-sm'))
                value = 'sm';
            return value;
        }
    }
    else{ 
        getSize = function(){
            var value = null;
            if (selectElement.classList.contains('custom-select-lg') || selectElement.classList.contains('form-control-lg'))
                value = 'lg';
            else if (selectElement.classList.contains('custom-select-sm') || selectElement.classList.contains('form-control-sm'))
                value = 'sm'; 
            return value;
        }
    }
    return getSize;
}

