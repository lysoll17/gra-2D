<?php
require_once "config.php";
$id = $_POST['id'];
$imie = $_POST['imie'];
$nazwisko = $_POST['nazwisko'];
$email = $_POST['email'];

$sql = "UPDATE uzytkownicy SET imie='$imie', nazwisko='$nazwisko', email='$email' WHERE id=$id";

if ($conn->query($sql)) {
    header("Location: index.php");
} else {
    echo "Błąd: " . $conn->error;
}
?>