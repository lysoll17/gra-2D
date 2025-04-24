<?php require_once "config.php"; ?>

<h2>Dodaj nowego użytkownika</h2>
<form method="POST" action="zapisz.php">
  Imię: <input type="text" name="imie" required><br>
  Nazwisko: <input type="text" name="nazwisko" required><br>
  Email: <input type="email" name="email" required><br>
  Hasło: <input type="password" name="haslo" required><br>
  <input type="submit" value="Zapisz">
</form>

<h2>Lista użytkowników</h2>
<table border="1">
  <tr><th>ID</th><th>Imię</th><th>Nazwisko</th><th>Email</th><th>Akcje</th></tr>

<?php
$result = $conn->query("SELECT * FROM uzytkownicy");
while ($row = $result->fetch_assoc()) {
    echo "<tr>
        <td>{$row['id']}</td>
        <td>{$row['imie']}</td>
        <td>{$row['nazwisko']}</td>
        <td>{$row['email']}</td>
        <td>
            <a href='edytuj.php?id={$row['id']}'>Edytuj</a> | 
            <a href='usun.php?id={$row['id']}'>Usuń</a>
        </td>
      </tr>";
}
?>
</table>