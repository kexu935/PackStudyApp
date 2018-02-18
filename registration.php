<?php
	//get all data necessary for database connection
	$ini_array = parse_ini_file("path.ini");
  	$path = $ini_array["URL"];
  	$host = $ini_array["HOST"];
  	$password = $ini_array["PASSWORD"];
	$dbusername = $ini_array["USERNAME"];
  	$dname = $ini_array["DNAME"];
	
	//get data that is posted from Registration activity

	
	$FirstName = $_POST["FirstName"];
	$LastName = $_POST["LastName"];
	$Email = $_POST["Email"];
	$Password = sha1($_POST["Password"]);
	$PhoneNumber = $_POST["PhoneNumber"];


	//create username from email
	$UsernameArr = explode("@", $Email);
	$Username = $UsernameArr[0];

	//PDO Connection to prevent MySQL injections
	$dsn = 'mysql:dbname='.$dname.';host='.$host.';charset=utf8mb4';
	$dbuser = $dbusername;
	$password = $password;
	$db = new PDO($dsn, $dbuser, $password);
	
	$data = array( 'FirstName' => $FirstName, 'LastName' => $LastName, 'Email' =>  $Email, 'Password' =>  $Password, 'PhoneNumber' =>  $PhoneNumber, 'Username' => $Username);

	$sth = $db->prepare('INSERT INTO Users (FirstName, LastName, Email, Password, PhoneNumber, Username) value (:FirstName, :LastName, :Email, :Password, :PhoneNumber, :Username)');

	//Check if the insert was sucessfull or unsucessfull
	if($sth->execute($data))
    {
     	echo "Sucess"; 
    }
	else
    {
     	echo "Error"; 
    }
	$db = null;
	
?>