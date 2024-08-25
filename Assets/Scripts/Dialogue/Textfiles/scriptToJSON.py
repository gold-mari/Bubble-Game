import os

def main():
    while True:
        inputString = input("Type DONE to terminate. Otherwise, what file should we convert? - ")

        if (inputString == "DONE"): 
            print("\n\n") 
            return

        dir_path = os.path.dirname(os.path.realpath(__file__))
        inputString = dir_path + "\\" + inputString
        print(f"Reading from {inputString}...")

        if not os.path.isfile(inputString):
            print("ERROR. File not found in local directory.")
            continue
        else:
            # Open file from path and start writing.
            print("File found!")
            outfilePath = input("What would you like to name the output file? - ")
            outfilePath = dir_path + "\\" + outfilePath
            convert(inputString, outfilePath)



def convert(infilePath, outfilePath):
    infile = open(infilePath, 'r')
    outfile = open(outfilePath, 'w')

    inLines = list(infile)

    header = '{\n\t"lines":\n\t[\n'
    outfile.writelines(header)

    # Pass 1: Validation and getting an actor list.
    actors = set();
    for i in range(len(inLines)):
        line = inLines[i]
        tokens = line.split(": ", 1)
        if len(tokens) != 2:
            raise SyntaxError(f"Error in convert(): token count on line {i} was not 2: {len(tokens)}")
            break;
        
        actors.add(tokens[0]);
    
    leftActors = set();
    rightActors = set();
    for a in actors:
        validAnswer = False
        while (not validAnswer):
            sideString = input(f"Which side of the screen is the actor named '{a}'? L or R - ")
            if (sideString == 'L'):
                validAnswer = True
                leftActors.add(a)
            if (sideString == 'R'):
                validAnswer = True
                rightActors.add(a)

    # Pass 2: Writing to JSON.
    currentLeft = ""
    currentRight = ""

    for i in range(len(inLines)):
        line = inLines[i]
        tokens = line.split(": ", 1)
        name = tokens[0]
        text = tokens[1].strip()

        block =  '\t\t{\n'
        block += '\t\t\t"actor":"' + name + '",\n'              # actor
        block += '\t\t\t"actions":"'                            # prepping for actions
        
        if (i == 0):
            firstLeft = ""
            firstRight = ""

            for j in range(len(inLines)):               # determine the first-front characters
                line = inLines[j]
                tokens = line.split(": ", 1)
                name = tokens[0]

                # if both were found, stop looking.
                if (firstLeft != "" and firstRight != ""):
                    break

                if (firstLeft == "" and name in leftActors):
                    firstLeft = name
                if (firstRight == "" and name in rightActors):
                    firstRight = name
            
            for actor in actors:                                # start back
                if actor == firstLeft: continue
                if actor == firstRight: continue
                block += actor + '.startBack,'

        else:
            if (name in leftActors and name != currentLeft):    # go back / go front
                block += currentLeft + '.goBack,'
                block += name + '.goFront,'
                currentLeft = name
            if (name in rightActors and name != currentRight):
                block += currentRight + '.goBack,'
                block += name + '.goFront,'
                currentRight = name

        block += name + '.sqBounce'                             # speaker squetch action
        for actor in actors:                                    # non-speaker squetch actions
            if actor == name: continue
            block += ',' + actor + '.sqIdle'
        block += '",\n'
        block += '\t\t\t"text":"' + text + '"\n'                # text

        if i == len(inLines)-1:
            block += '\t\t}\n'
        else:
            block += '\t\t},\n'
    
        outfile.writelines(block)

    footer = '\t]\n}'
    outfile.writelines(footer)            

    infile.close()
    outfile.close()    

if __name__ == "__main__":
    main()