<?php
require_once "config.php";
$id = $_GET['id'];

$conn->query("DELETE FROM uzytkownicy WHERE id = $id");
header("Location: index.php");