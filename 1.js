function documentSetup() {//function <- <-
    document.getElementById('e').addEventListener('click', function() {
        window.location.href = 'https://web.whatsapp.com'; // Change the current window's location
    });
}

documentSetup(); // Call the function to set up the event listener