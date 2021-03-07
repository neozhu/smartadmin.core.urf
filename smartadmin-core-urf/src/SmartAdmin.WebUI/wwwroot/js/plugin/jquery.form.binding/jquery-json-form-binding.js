$.fn.jsonToForm = function (data, callbacks) {

    var formInstance = this;

    var options = {

        data: data || null,
        callbacks: callbacks

    };

     if (options.data != null) {
       $.each(options.data, function (k, v) {
            var elements = $('[name="' + k + '"]', formInstance); 
            
            if (options.callbacks != null && options.callbacks.hasOwnProperty(k)) {

                options.callbacks[k](v);

            } else if (elements.length || (v != undefined)) {
                // check number of occurance of the element, 
                // radio group / checkbox element will be having same name and hence multiple occurance will be there inside a form
                $(elements).each(function (index, ele) {
                    if (Array.isArray(v)) { // checkbox group and multiple select will have array of values
                        v.forEach(function (val) {
                            $(ele).is("select") // check if select control
                                ? $(ele)
                                    .find("[value='" + val + "']")
                                    .prop("selected", true) // select option if select control
                                : ($(ele).val() == val)
                                    ? $(ele).prop("checked", true) // mark as checked if radio or checkbox
                                    : "";
                        });
                    } else if ($(ele).is(':checkbox') || $(ele).is(':radio')) {  // checkbox group or radio group
                        ($(ele).val() == v) ? $(ele).prop("checked", true) : "";
                    } else {
                        $('[name="' + k + '"]', formInstance).val(v);
                    }
                });
           
                    
            }

        });
    }

}
