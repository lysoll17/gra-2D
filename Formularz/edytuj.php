<?php
require_once "config.php";
$id = $_GET['id'];
$result = $conn->query("SELECT * FROM uzytkownicy WHERE id = $id");
$row = $result->fetch_assoc();
?>

<h2>Edytuj użytkownika</h2>
<form method="POST" action="aktualizuj.php">
  <input type="hidden" name="id" value="<?= $row['id'] ?>">
  Imię: <input type="text" name="imie" value="<?= $row['imie'] ?>"><br>
  Nazwisko: <input type="text" name="nazwisko" value="<?= $row['nazwisko'] ?>"><br>
  Email: <input type="email" name="email" value="<?= $row['email'] ?>"><br>
  <input type="submit" value="Aktualizuj">
</form>