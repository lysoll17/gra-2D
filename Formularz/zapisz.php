<?php
require_once "config.php";

$imie = $_POST['imie'];
$nazwisko = $_POST['nazwisko'];
$email = $_POST['email'];
$haslo = password_hash($_POST['haslo'], PASSWORD_DEFAULT);

$sql = "INSERT INTO uzytkownicy (imie, nazwisko, email, haslo)
        VALUES ('$imie', '$nazwisko', '$email', '$haslo')";

if ($conn->query($sql)) {
    echo "Dodano użytkownika. <a href='index.php'>Powrót</a>";
} else {
    echo "Błąd: " . $conn->error;
}
?>