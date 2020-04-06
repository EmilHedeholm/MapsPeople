function validate() {
    var error = false;
    var errorText = "";
    var errorName = false;
    var numberOfErrors = 0;
    var numberOfErrorsProcessed = 0;
    // Queue ID
    var inputQueue = "";
    try {
        inputQueue = document.chooseQueue.queueName.value;
    }
    catch (err) { return true; }
    if (inputQueue.length < 1) {
        error = true;
        errorName = true;
        errorText = "Input cannot be empty";
        numberOfErrors++;
    }
    if (error) {
        if (numberOfErrors > 1) {
            errorText = "Search for a queue ID";
            if (errorName) {
                errorText += " ID";
                numberOfErrorsProcessed++;
            }
        }
        alert(errorText);
        return false;
    }
    return true;
}