# Project

This is a project for handling parking slots and make slot reservations.

# Installation

1) Clone this repo.
2) Run the following command: ```docker-compose up --build```
3) Open ```localhost:3000```  in your browser.

# Usage
* On installation, the database is seeded with example data.
* **User Selection**: In the top navigation bar, in a dropdown field, you can select in which user's name you want to use the site in. Reservation and cancellation options might differ among users. The icons next to the user's name display what special type of parking slots the user may use. 
* **Reservation**: On the *Dashboard* page, find a parking slot and click on the *Book slot* button. Select the date, and the starting and ending times, and send the reservation request.
* **Cancellation**: Navigate to the my reservations page. For every reservation that has not started yet, you can cancel the reservations by clicking on the *Cancel* button. The same functionality is available on the Dashboard page, only with your own reservations.