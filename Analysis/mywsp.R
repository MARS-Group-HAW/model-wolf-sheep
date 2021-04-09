sheeps <- read.csv(file="../WolfSheepPredation/bin/Debug/netcoreapp2.0/Sheep.csv",head=T, sep=";", dec="." )
wolves <- read.csv(file="../WolfSheepPredation/bin/Debug/netcoreapp2.0/Wolf.csv",head=T, sep=";", dec="." )

attach(sheeps)
attach(wolves)

max_sheep_steps <- max(sheeps$Step)
max_wolf_steps <- max(wolves$Step)

sheep_pop <- matrix(nrow = 1, ncol = max_sheep_steps)
wolf_pop <- matrix(nrow = 1, ncol = max_wolf_steps)

for (st in c(0:max_sheep_steps)) {
  sheep_pop[st] <- nrow(subset(sheeps,Step==st))
}

for (st in c(0:max_wolf_steps)) {
  wolf_pop[st] <- nrow(subset(wolves,Step==st))
}

max_sheep <- max(sheep_pop)
max_wolf <- max(wolf_pop)

jpeg(filename = "mywsp.jpg",width = 1200, height = 800)

plot(c(1:max(max_sheep_steps,max_wolf_steps)), ylim = c(0,max(max_sheep,max_wolf)), type = "n", 
     xlab ="Simulation steps", ylab = "Population")
box()
lines(wolf_pop[1,],col="darkgrey")
lines(sheep_pop[1,], col="blue")
legend("topleft",legend=c("Wolf", "Sheep"), col=c("darkgrey", "blue"), lty = 1)
dev.off()
