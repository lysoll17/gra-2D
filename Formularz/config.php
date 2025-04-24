<?php
$conn = new mysqli("localhost", "root", "", "szkola");
if ($conn->connect_error) {
    die("Błąd połączenia z bazą danych: " . $conn->connect_error);
}
?>