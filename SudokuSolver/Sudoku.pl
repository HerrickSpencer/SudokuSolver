#! perl -w

### Need to add a check for 'singles'  ones where it is the only one in the set with that number...

open BOARD, $ARGV[0];
my @board = <BOARD>;
chomp @board;
#eval{map {s/\D//g;} @board;};
foreach (@board){
	$_=~s/[^0-9\s]//g;
	$_=[split(//,$_)];
}
	
&PrintBoard;
my @debug = (1,6);

#fill spaces with all possibilities
for (my $y = 0; $y < 9; $y++){
	for (my $x = 0; $x < 9; $x++){
		$board[$y][$x]=~s/ /123456789/;
	}
}	

#run till all are 1 number
for (0..100){
	my $working = 0;
	#loop through board reducing any square that's unsolved
	for (my $y = 0; $y < 9; $y++){
		for (my $x = 0; $x < 9; $x++){
			if (length($board[$y][$x])>1){
				$working+=(&Reduce($y,$x));
#			print "DDD $y/$x space=$board[$y][$x] " if ($y == $debug[0] && $x == $debug[1]);
			}
		}
	}	
	print "Finished:\n" unless $working;
	&PrintBoard unless $working;
	last unless $working;
}

#print board
sub PrintBoard{
	for (my $y = 0; $y < 9; $y++){
		for (my $x = 0; $x < 9; $x++){
			print $board[$y][$x];
			print "|" unless $x==8;
		}
		print "\n";
	}
	print "---------------\n";
}	

sub Reduce{
	#removes any solved number from this square
	my ($y,$x)=@_;
	my $initial = $board[$y][$x];
	return 0 if length($initial) == 1;

	#SoleSurvivor checks for the only option left for:col/row/section
	my @soleSurvivor=("123456789","123456789","123456789");
	my @foundDup=(0,0,0);
	for (my $i = 0; $i < 9; $i++){
		#remove by column
		my $pattern = $board[$i][$x];
#		print "C $y/$x vs $i/$x space=$board[$y][$x] pattern = $pattern\n" if ($y == $debug[0] && $x == $debug[1] && length($pattern)==1);
		$board[$y][$x]=~s/$pattern// if ($i!=$y && length($pattern)==1);
		$soleSurvivor[0]=~s/[$pattern]// if ($i!=$y && length($pattern)>1);
		$foundDup[0] = 1 if ($pattern eq $board[$y][$x] && length($pattern)==2);

		#remove by row
		$pattern = $board[$y][$i];
#		print "R $y/$x vs $y/$i space=$board[$y][$x] pattern = $pattern\n" if ($y == $debug[0] && $x == $debug[1] && length($pattern)==1);
		$board[$y][$x]=~s/$pattern// if ($i!=$x && length($pattern)==1);
		$soleSurvivor[1]=~s/[$pattern]// if ($i!=$x && length($pattern)>1);
		$foundDup[1] = 1 if ($pattern eq $board[$y][$x] && length($pattern)==2);

	}	

	#remove by section (3x3areas)
	foreach (($y-$y%3)..($y-$y%3)+2){
		my $y1=$_;
		foreach (($x-$x%3)..($x-$x%3)+2){
			my $x1=$_;
			next if ($y==$y1 && $x==$x1);
			my $pattern = $board[$y1][$x1];
#			print "S $y/$x vs $y1/$x1 space=$board[$y][$x] pattern = $pattern\n" if ($y == $debug[0] && $x == $debug[1] && length($pattern)==1);
			$board[$y][$x]=~s/$pattern// if (length($pattern)==1);
			$soleSurvivor[2]=~s/[$pattern]// if (length($pattern)>1);
			$foundDup[2] = 1 if ($pattern eq $board[$y][$x] && length($pattern)==2);
		}
	}

	#Check for last option for Col/Row/Section
	for (my $i = 0; $i <= $#soleSurvivor; $i++){
		last if (length($board[$y][$x])==1);
		if (length($soleSurvivor[$i])==1){
#			print "L $y/$x sole $_---".length($soleSurvivor[$i])."\n" if ($y == $debug[0] && $x == $debug[1]);
			$board[$y][$x]=$soleSurvivor[$i];
			last;
		}
	}

	my $didReduce = ($initial ne $board[$y][$x])?1:0;

	#Check for doubleJeopardy
	for (my $i=0;$i<@foundDup;$i++){
		next unless ($foundDup[$i] && length($board[$y][$x])==1);
		if ($i==0){
			foreach(0..8){
				next if ($board[$_][$x] eq $board[$y][$x]);
				$didReduce++ if ($board[$_][$x]=~s/[$board[$y][$x]]//);
			}
		}
		if ($i==1){
			foreach(0..8){
				next if ($board[$y][$_] eq $board[$y][$x]);
				$didReduce++ if ($board[$y][$_]=~s/[$board[$y][$x]]//);
			}
		}

		if ($i==2){
			foreach (($y-$y%3)..($y-$y%3)+2){
				my $y1=$_;
				foreach (($x-$x%3)..($x-$x%3)+2){
					my $x1=$_;
					next if ($board[$y1][$x1] eq $board[$y][$x]);
					$didReduce++ if ($board[$y1][$x1]=~s/[$board[$y][$x]]//);
				}
			}
		}
	}	


	#tell caller if it reduced or not
	print $didReduce if ($didReduce);
	return $didReduce; #($initial ne $board[$y][$x])?1:0;
}

#sub CheckDoubles{
#	for (0..9){
#		my $y=$_;
#		for (0..9){
#			my $x=$_;
#			next if ($x
#		}
#	}
#}