<?php
	//get all data necessary for database connection
	$ini_array = parse_ini_file("path.ini");
  	$path = $ini_array["URL"];
  	$host = $ini_array["HOST"];
  	$password = $ini_array["PASSWORD"];
	$dbusername = $ini_array["USERNAME"];
  	$dname = $ini_array["DNAME"];
	
	//get data that is posted from Registration activity

	
	$UserId = $_POST["UserId"];
	$Course = $_POST["CourseId"];
	$Semester = $_POST["SemesterId"];
	
	$UserIdInt = (int)$UserId;
	//create username from email
	$UsernameArr = explode("@", $Email);
	$Username = $UsernameArr[0];

	//PDO Connection to prevent MySQL injections
	$dsn = 'mysql:dbname='.$dname.';host='.$host.';charset=utf8mb4';
	$dbuser = $dbusername;
	$password = $password;
	$db = new PDO($dsn, $dbuser, $password);
	
	$data = array( 'UserId' => $UserIdInt, 'Semester' => $Semester,  'Course' => $Course);

	$sth = $db->prepare('INSERT INTO Enrollment (UserId, Semester, Course) value (:UserId, :Semester, :Course)');

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