
cairo_pdf(filename = "../src-gen/wolf-sheep-population.pdf")
sheeps <- read.csv(file="../src-gen/Sheep.csv",head=T, sep=";", dec="." )
wolves <- read.csv(file="../src-gen/Wolf.csv",head=T, sep=";", dec="." )

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

# par(bg="black")
plot(c(1:max(max_sheep_steps,max_wolf_steps)), ylim = c(0,max(sheep_pop,wolf_pop)), type = "n", xlab ="Simulation steps", ylab = "Population")
lines(wolf_pop[1,],col="darkgrey")
lines(sheep_pop[1,], col="blue")
legend("topright",legend=c("Wolf", "Sheep"), col=c("darkgrey", "blue"),lty=1)
dev.off()
